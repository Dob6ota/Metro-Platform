using UnityEngine;
using UnityEngine.Events;

namespace MetroPlatform
{
    public class Sensor : DebuggableMonoBehaviour
    {
        [SerializeField] private Collider2D _collider;

        [SerializeField] private UnityEvent _activated;
        void Start()
        {

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Debugging)
            {
                Debug.Log($"Sensor OnTriggerEnter2D");
            }

            var demageable = other.GetComponent<IDamageable>();

            if (demageable != null)
            {
                if (Debugging)
                {
                    Debug.Log($"Sensor damageable: {demageable.GetType().ToString()}");
                }
                _activated.Invoke();
            }
        }
    }
}
