using System.Collections;
using UnityEngine;

namespace MetroPlatform
{
    public class SelfDestruction : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 0.25f;
        private void Start()
        {
            StartCoroutine(DestroyItself(_lifeTime));
        }
  
        private IEnumerator DestroyItself(float lifeTime)
        {
            yield return new WaitForSeconds(lifeTime); 
            Destroy(gameObject);
        }
    }
}
