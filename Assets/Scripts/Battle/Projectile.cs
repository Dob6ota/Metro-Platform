using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace MetroPlatform
{
    public class Projectile : DebuggableMonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _damage = 6.0f;
        [SerializeField] private float _speed = 6.0f;
        [SerializeField] private Vector3 _direction = Vector3.right;

        [Header("Refs")]
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Collider2D _colliderProj;
        [SerializeField] private SelfDestruction _selfDestruction;

        private void Awake()
        {
            if (_colliderProj == null)
            {
                _colliderProj = GetComponent<Collider2D>();
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.linearVelocity = _direction.normalized * _speed;
        }

        internal void Initialize(float speed, Vector3 direction, Collider2D colliderSpawner = null)
        {
            _speed = speed;
            _direction = direction;

            if (direction.x > 0)
                transform.localEulerAngles = Vector3.zero;
            else if (direction.x < 0)
                transform.localEulerAngles = new Vector3(0, -180, 0);
            if (direction.y > 0)
                transform.localEulerAngles = Vector3.zero;
            else if (direction.y < 0)
                transform.localEulerAngles = new Vector3(-180, 0, 0);

            if (colliderSpawner != null)
            {
                Physics2D.IgnoreCollision(colliderSpawner, _colliderProj);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Debugging)
            {
                Debug.Log($"Projectile OnTriggerEnter2D");
            }

            _speed = 0f;
            _selfDestruction.enabled = true;   
            var demageable = other.GetComponent<IDamageable>();

            if (demageable != null)
            {
                if (Debugging)
                {
                    Debug.Log($"Projectile damageable: {demageable.GetType().ToString()}");
                }
                demageable.ReceiveDamage(_damage);
            }
        }
    }
}
