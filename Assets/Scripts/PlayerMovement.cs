using UnityEngine;

namespace MetroPlatform
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Rigidbody2D _rb;
        private void Start()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = new Vector2(_playerInput.InputAxisX, 0f);
        }
    }
}
