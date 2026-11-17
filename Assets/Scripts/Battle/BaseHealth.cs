using UnityEngine;
using UnityEngine.Events;

namespace MetroPlatform
{
    public class BaseHealth : DebuggableMonoBehaviour, IDamageable
    {
        [SerializeField] private HealthBank _currentHealth;
        [SerializeField] private HealthStats _healthStats;

        public UnityEvent OnDie;

        public float HealthNorm => _currentHealth.HitPoints / _healthStats.MaxHealth.HitPoints;
        void Start()
        {
            _currentHealth.HitPoints = _healthStats.MaxHealth.HitPoints;
        }

        public void ReceiveDamage(float damage)
        {
            if (Debugging)
            {
                Debug.Log($"BaseHealth ReciveDamage damage {damage}", this);
            }

            _currentHealth.HitPoints -= damage;

            if (_currentHealth.HitPoints <= 0) 
            {
                if (Debugging)
                {
                    Debug.Log($"BaseHealth ReciveDamage dying", this);
                }
                OnDie.Invoke();

                Destroy(gameObject);
            }
        }
    }
}
