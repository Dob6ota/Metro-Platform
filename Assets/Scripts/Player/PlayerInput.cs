using UnityEngine;

namespace MetroPlatform
{
    public class PlayerInput : MonoBehaviour
    {
        public float InputAxisX;
        void Start()
        {
        
        }

        void Update()
        {
            InputAxisX = Input.GetAxisRaw("Horizontal");
        }
    }
}
