using UnityEngine;

namespace MetroPlatform
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Setting")]
        [SerializeField] private float _walkSpeed = 6f;

        [Header("Jump Setting")]
        [SerializeField] private float _jumpForce = 9f;
        [SerializeField] private LayerMask _jumpLayerMask;
        [SerializeField] private float _jumpTimeTotal = 0.15f;
        [SerializeField] private float _inputBufferJumpTimeTotal = 0.15f;
        [SerializeField] private float _inputBufferJumpTimeCounter;

        [SerializeField] private float _coyoteTimeTotal = 0.1f;
        [SerializeField] private float _coyoteTimeCounter;


        [Header("Inner Working")]
        [SerializeField] private float _moveHorizontal;
        [SerializeField] private bool _canJump;
        [SerializeField] private bool _grounded;
        [SerializeField] private float _moveVertical;
        [SerializeField] private bool _jumping;
        [SerializeField] private float _jumpCounter;


        private float _inputAxisX => _playerInput.InputAxisX;
        private bool _inputJump => _playerInput.InputJump;

        [Header("Refs")]
        [SerializeField] private PlayerInput _playerInput; //ссылки - поле
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Transform _groundCheckT;


        private void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            if (_playerInput == null)
                _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            InputBufferJump(_inputJump);

            OnJump(_inputBufferJumpTimeCounter > 0);

            _canJump = CanJump();

            if (_jumping)
            {
                _jumpCounter -= Time.deltaTime;
                if (_jumpCounter <= 0)
                {
                    _jumping = false;
                }
            }
        }

        private void FixedUpdate()
        {
            _moveHorizontal = FixedMove(_inputAxisX);

            _moveVertical = FixedJump(_jumping);

            _rb.linearVelocity = new Vector2(_moveHorizontal, _moveVertical);
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
                _coyoteTimeCounter = _coyoteTimeTotal;
            }
            else if (_coyoteTimeCounter > 0)
            {
                _coyoteTimeCounter -= Time.deltaTime;
            }
            return _coyoteTimeCounter > 0;
        }

        private float FixedMove(float inputAxisX) 
        {
            return inputAxisX * _walkSpeed;
        }

        private float FixedJump(bool jumping)
        {
            if (jumping)
            {
                return _jumpForce;
            }

            return 0f;
        }

        private void OnJump(bool inputJump)
        {
            if (inputJump && _canJump)
            {
                _jumping = true;
                _jumpCounter = _jumpTimeTotal;
            }
        }

        private bool Grounded()
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(_groundCheckT.position, Vector2.down, 0.05f, _jumpLayerMask);
            
            return raycastHit.collider != null;
        }
    }
}
