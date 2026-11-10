using UnityEngine;

namespace MetroPlatform
{
    public class Box : MonoBehaviour, IDamageable
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        public void ReceiveDamage(float damage)
        {
            Destroy(gameObject);
        }
    }
}
