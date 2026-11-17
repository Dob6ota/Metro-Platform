using Unity.Cinemachine;
using UnityEngine;

namespace MetroPlatform
{
    public class Spikes : DebuggableMonoBehaviour
    {
        [SerializeField] private float _damage = 10f;
        private void Start()
        {
        
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Debugging)
            {
                Debug.Log($"Spikes OnTriggerEnter2D");
            }

            var demageable = other.GetComponent<IDamageable>();

            if (demageable != null)
            {
                if (Debugging)
                {
                    Debug.Log($"Spikes damageable: {demageable.GetType().ToString()}");
                }
                demageable.ReceiveDamage(_damage);
            }
        }
    }
}
