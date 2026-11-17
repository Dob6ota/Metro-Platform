using UnityEngine;
using UnityEngine.UI;

namespace MetroPlatform
{
    public class UIPlayerHealth : MonoBehaviour
    {
        [SerializeField] private Slider _playerHealthSlider;
        [SerializeField] private BaseHealth _health;

        void Start()
        {
        
        }

        void Update()
        {
            _playerHealthSlider.value = _health.HealthNorm;
        }
    }
}
