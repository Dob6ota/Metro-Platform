using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetroPlatform
{
    // Наследуемся от DebuggableMonoBehaviour, чтобы получить доступ к галочке Debugging и методу ConditDebugLog
    public class PlayerMovement : DebuggableMonoBehaviour
    {

        [Header("Movement Setting")]
        [SerializeField] private float _walkSpeed = 6f;

        [Header("Jump Setting")]
        [SerializeField] private float _jumpForce = 9f; 
        [SerializeField] private float _jumpTimeTotal = 0.15f;  // Время прыжка персонажа вверх
        [SerializeField] private LayerMask _jumpLayerMask;      // Слой для рейкаста
        [SerializeField] private float _jumpFallAcceleration = 0.3f; // Ускорение свободного падения

        // Буфер прыжка: позволяет нажать прыжок чуть раньше приземления, и он засчитается
        [SerializeField] private float _inputBufferJumpTimeTotal = 0.15f;
        [SerializeField] private float _inputBufferJumpTimeCounter;

        // Coyote Time: позволяет прыгнуть сразу после того, как персонаж сбежал с края платформы
        [SerializeField] private float _coyoteTimeTotal = 0.1f;

        [Header("Inner Working")]
        [SerializeField] private float _moveHorizontal;       // Текущая горизонтальная скорость
        [SerializeField] private float _jumpFallForceActing;  // Текущая скорость падения (накапливается со временем)
        [SerializeField] private bool _canJump;               
        [SerializeField] private bool _grounded;             
        [SerializeField] private float _moveVertical;         // Текущая вертикальная скорость

        // Ссылки на ввод
        private float _inputAxisX => _playerInput.InputAxisX;
        private bool _inputJump => _playerInput.InputJump;

        // Таймеры для управления состояниями
        [SerializeField] private TimerCountdown _timerJump;
        [SerializeField] private TimerCountdown _timerFall;
        [SerializeField] private TimerCountdown _timerCoyoteTime;
        [SerializeField] private List<TimerCountdown> _timers; // Список всех таймеров для удобного обновления

        [Header("Refs")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Transform _visuals;      // Объект с картинкой
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Transform _groundCheckT; // Точка под ногами для проверки земли

        // Хэши параметров аниматора (используются для оптимизации, чтобы не передавать строки каждый кадр)
        private static readonly int AnimVelocityX = Animator.StringToHash("VelocityX");
        private static readonly int AnimJumping = Animator.StringToHash("Jumping");
        private static readonly int AnimFalling = Animator.StringToHash("Falling");

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_animator == null) _animator = _visuals.GetComponent<Animator>();
            if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();

            InitializeTimers();
        }

        private void Update()
        {
            // Обработка ввода прыжка (буферизация)
            InputBufferJump(_inputJump);

            // Попытка прыгнуть, если в буфере есть запас времени
            OnJump(_inputBufferJumpTimeCounter > 0);

            // Проверка, можно ли прыгать (стоим на земле или в состоянии Coyote Time)
            _canJump = CanJump();

            // Обновление всех активных таймеров (вычитание времени)
            UpdateTimers();

            // Визуал (анимация и поворот)
            Animate();
            Flip(_inputAxisX);
        }

        private void FixedUpdate()
        {
            _moveHorizontal = FixedMove(_inputAxisX);

            // Логика вертикального движения зависит от того, какой таймер сейчас активен
            if (_timerJump.IsRunning)
            {
                _moveVertical = FixedJump();
                ConditDebugLog($"_moveVertical {_moveVertical}");
            }
            else if (_timerFall.IsRunning)
            {
                _moveVertical = FixedFall();
                ConditDebugLog($"_moveVertical {_moveVertical}");
            }

            // Проверяем, не пора ли остановить таймеры (например, если мы приземлились)
            ChangeTimersState();

            // Применяем итоговую скорость к Rigidbody2D
            _rb.linearVelocity = new Vector2(_moveHorizontal, _moveVertical);
        }

        private void InitializeTimers()
        {
            // Создаем экземпляры таймеров
            _timerJump = new TimerCountdown(_jumpTimeTotal, "_timerJump");
            if (Debugging) _timerJump.Debugging = true; // Включаем логи для прыжка, если включен общий дебаг

            _timerFall = new TimerCountdown(999f, "_timerFall"); // Таймер падения фактически бесконечный, пока не приземлимся
            _timerCoyoteTime = new TimerCountdown(_coyoteTimeTotal, "_timerCoyoteTime");

            // подписка на события
            // Когда прыжок начинается -> включаем анимацию прыжка
            _timerJump.TimerStart += () => {
                _animator.SetBool(AnimJumping, _timerJump.IsRunning);
            };

            // Когда прыжок заканчивается -> начинаем падать, переключаем анимации
            _timerJump.TimerStop += () => {
                _timerFall.Start();
                _animator.SetBool(AnimJumping, _timerJump.IsRunning);
                _animator.SetBool(AnimFalling, _timerFall.IsRunning);
            };

            // Когда падение начинается -> сбрасываем скорость падения в ноль
            _timerFall.TimerStart += () => _jumpFallForceActing = 0;

            // Когда падение заканчивается (приземлились) -> выключаем анимацию падения
            _timerFall.TimerStop += () => {
                _animator.SetBool(AnimFalling, _timerFall.IsRunning);
            };

            // Складываем все таймеры в список для удобного тика
            _timers = new List<TimerCountdown>(3) { _timerJump, _timerFall, _timerCoyoteTime };
        }

        private void UpdateTimers()
        {
            // Проходимся по списку и тикаем только те таймеры, которые запущены
            foreach (var timer in _timers)
            {
                if (timer.IsRunning)
                {
                    ConditDebugLog($"timer {timer.Name} is Running");
                    timer.Tick(Time.deltaTime);
                }
            }
        }

        private void Animate()
        {
            // Передаем модуль скорости в аниматор (чтобы анимация бега не зависела от направления)
            _animator.SetFloat(AnimVelocityX, Mathf.Abs(_inputAxisX));
        }

        private void Flip(float inputAxisX)
        {
            // Поворачиваем визуал в зависимости от направления движения
            if (inputAxisX != 0)
            {
                if (inputAxisX > 0) transform.localEulerAngles = Vector3.zero;
                else transform.localEulerAngles = new Vector3(0, -180, 0);
            }
        }

        private void ChangeTimersState()
        {
            // Если мы падаем и вдруг оказались на земле -> останавливаем таймер падения
            if (_timerFall.IsRunning && _grounded)
            {
                _timerFall.Stop();
            }

            // Если мы в прыжке, но уже на земле (например, ударились головой или приземлились раньше),
            // и прогресс прыжка больше 30% -> принудительно останавливаем прыжок
            if (_timerJump.IsRunning && _grounded && _timerJump.Progress > 0.3f)
            {
                _timerJump.Stop();
            }
        }

        private float FixedMove(float inputAxisX)
        {
            // Банальное вычисление горизонтальной скорости
            return inputAxisX * _walkSpeed;
        }

        private float FixedJump()
        {
            // Магия плавного прыжка: умножаем силу на ProgressReversed.
            // В начале ProgressReversed = 1 (полная сила), к концу таймера = 0 (сила падает до нуля).
            return _jumpForce * _timerJump.ProgressReversed;
        }

        private float FixedFall()
        {
            // Кастомная гравитация: каждый кадр уменьшаем скорость (делаем её более отрицательной)
            _jumpFallForceActing -= _jumpFallAcceleration;
            return _jumpFallForceActing;
        }

        private void InputBufferJump(bool inputJump)
        {
            // Если игрок нажал кнопку -> обнуляем счетчик буфера
            if (inputJump)
            {
                _inputBufferJumpTimeCounter = _inputBufferJumpTimeTotal;
            }
            // Если не нажал, но счетчик еще не истек -> тикаем его вниз
            else if (_inputBufferJumpTimeCounter > 0)
            {
                _inputBufferJumpTimeCounter -= Time.deltaTime;
            }
        }

        private bool CanJump()
        {
            _grounded = Grounded(); // Проверяем, стоим ли на земле

            // Если на земле -> постоянно обновляем таймер Coyote Time
            if (_grounded)
            {
                _timerCoyoteTime.Reset();
                _timerCoyoteTime.Start();
            }

            // Прыгнуть можно, если таймер Coyote Time активен и мы еще не в прыжке
            return _timerCoyoteTime.IsRunning && !_timerJump.IsRunning;
        }

        private void OnJump(bool inputJump)
        {
            // Если есть буфер ввода, мы можем прыгать и не прыгаем в данный момент -> запускаем прыжок
            if (inputJump && _canJump && !_timerJump.IsRunning)
            {
                _timerJump.Start(_jumpTimeTotal);
            }
        }

        private bool Grounded()
        {
            // Пускаем луч вниз из точки под ногами длиной 0.1f
            RaycastHit2D raycastHit = Physics2D.Raycast(_groundCheckT.position, Vector2.down, 0.1f, _jumpLayerMask);

            // Рисуем луч в редакторе (зеленый, если попали в землю, красный - если нет)
            Debug.DrawRay(_groundCheckT.position, Vector2.down * 0.1f, raycastHit.collider != null ? Color.green : Color.red);

            // Если луч во что-то попал (collider != null) -> мы на земле
            return raycastHit.collider != null;
        }
    }
}