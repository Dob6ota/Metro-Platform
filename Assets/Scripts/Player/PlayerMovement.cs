using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetroPlatform
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Setting")]
        [SerializeField] private float _walkSpeed = 6f;

        [Header("Jump Setting")]
        [SerializeField] private float _jumpForce = 9f;
        [SerializeField] private float _jumpTimeTotal = 0.15f;

        [SerializeField] private LayerMask _jumpLayerMask;

        [SerializeField] private float _jumpFallAcceleration = 0.3f;

        [SerializeField] private float _inputBufferJumpTimeTotal = 0.15f;
        [SerializeField] private float _inputBufferJumpTimeCounter;

        [SerializeField] private float _coyoteTimeTotal = 0.1f;


        [Header("Inner Working")]

        [SerializeField] private float _moveHorizontal;

        [SerializeField] private float _jumpFallForceActing;
        [SerializeField] private bool _canJump;
        [SerializeField] private bool _grounded;
        [SerializeField] private float _moveVertical;
        //[SerializeField] private bool _jumping;
        //[SerializeField] private float _jumpCounter;


        private float _inputAxisX => _playerInput.InputAxisX;
        private bool _inputJump => _playerInput.InputJump;

        // Timers
        [SerializeField] private TimerCountdown _timerJump;
        [SerializeField] private TimerCountdown _timerFall;
        [SerializeField] private TimerCountdown _timerCoyoteTime;
        [SerializeField] private List<TimerCountdown> _timers;

        [Header("Refs")]
        [SerializeField] private PlayerInput _playerInput; //ссылки - поле
        [SerializeField] private Transform _visuals;
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Transform _groundCheckT;

        private static readonly int AnimVelocityX = Animator.StringToHash("VelocityX");
        private static readonly int AnimJumping = Animator.StringToHash("Jumping");
        private static readonly int AnimFalling = Animator.StringToHash("Falling");


        private void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            if (_animator == null)
                _animator = _visuals.GetComponent<Animator>();
            if (_playerInput == null)
                _playerInput = GetComponent<PlayerInput>();

            InitializeTimers();
        }

        private void InitializeTimers()
        {
            _timerJump = new TimerCountdown(_jumpTimeTotal, "_timerJump");
            _timerJump.Debugging = true;
            _timerFall = new TimerCountdown(999f, "_timerFall");
            _timerCoyoteTime = new TimerCountdown(_coyoteTimeTotal, "_timerCoyoteTime");

            _timerJump.TimerStart += () => {
                _animator.SetBool(AnimJumping, _timerJump.IsRunning);
            };
            _timerJump.TimerStop += () => { 
                _timerFall.Start();
                _animator.SetBool(AnimJumping, _timerJump.IsRunning);
                _animator.SetBool(AnimFalling, _timerFall.IsRunning);

            };
            _timerFall.TimerStart += () => _jumpFallForceActing = 0;
            _timerFall.TimerStop += () => {
                _animator.SetBool(AnimFalling, _timerFall.IsRunning);
            };

            _timers = new List<TimerCountdown>(3) {_timerJump, _timerFall, _timerCoyoteTime };
        }

        private void Update()
        {
            InputBufferJump(_inputJump);

            OnJump(_inputBufferJumpTimeCounter > 0);

            _canJump = CanJump();

            UpdateTimers();

            Animate();
            Flip(_inputAxisX);
        }

        private void UpdateTimers()
        {
            foreach (var timer in _timers)
            {
                if (timer.IsRunning)
                {
                    Debug.Log($"timer {timer.Name} is Running");
                    timer.Tick(Time.deltaTime);
                }
            }
        }

        private void FixedUpdate()
        {
            _moveHorizontal = FixedMove(_inputAxisX);

            if (_timerJump.IsRunning)
            {
                _moveVertical = FixedJump();
                Debug.Log($"_moveVertical {_moveVertical}");
            }

            else if (_timerFall.IsRunning)
            {
                _moveVertical = FixedFall();
                Debug.Log($"_moveVertical {_moveVertical}");
            }

            ChangeTimersState();

            _rb.linearVelocity = new Vector2(_moveHorizontal, _moveVertical);
        }

        private void Animate()
        {
            _animator.SetFloat(AnimVelocityX, Mathf.Abs(_inputAxisX));;
        }

        private void Flip(float inputAxisX)
        {
            if (inputAxisX != 0)
            {
                if (inputAxisX > 0)
                {
                    transform.localEulerAngles = Vector3.zero;
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, -180, 0);
                }
            }

        }

        private void ChangeTimersState()
        {
            if (_timerFall.IsRunning && _grounded)
            {
                _timerFall.Stop();
            }

            if (_timerJump.IsRunning && _grounded && _timerJump.Progress > 0.3f)
            {
                _timerJump.Stop();

            }
        }

        private void InputBufferJump(bool inputJump)
        {
            if (inputJump)
            {
                _inputBufferJumpTimeCounter = _inputBufferJumpTimeTotal;
            }
            else if (_inputBufferJumpTimeCounter > 0)
            {
                _inputBufferJumpTimeCounter -= Time.deltaTime;
            }
        }

        private bool CanJump()
        {
            _grounded = Grounded();

            if (_grounded)
            {
                _timerCoyoteTime.Reset();
                _timerCoyoteTime.Start();
            }

            return _timerCoyoteTime.IsRunning && !_timerJump.IsRunning;
        }

        private float FixedMove(float inputAxisX) 
        {
            return inputAxisX * _walkSpeed;
        }

        private float FixedJump()
        {
            return _jumpForce * _timerJump.ProgressReversed;
        }

        private float FixedFall()
        {
            _jumpFallForceActing -= _jumpFallAcceleration;
            return _jumpFallForceActing;
        }

        private void OnJump(bool inputJump)
        {
            if (inputJump && _canJump && !_timerJump.IsRunning)
            {
                _timerJump.Start(_jumpTimeTotal);
            }
        }

        private bool Grounded()
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(_groundCheckT.position, Vector2.down, 0.1f, _jumpLayerMask);
            Debug.DrawRay(_groundCheckT.position, Vector2.down * 0.1f, raycastHit.collider != null ? Color.green : Color.red);

            return raycastHit.collider != null;
        }

        private void OnTimerJumpStopped()
        {
            _timerFall.Start();
        }
    }
}
