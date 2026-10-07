using UnityEngine;

namespace MetroPlatform
{
    public class PlayerInput : MonoBehaviour
    {
        public float InputAxisX;
        public bool InputJump;
        void Start()
        {
        
        }

        void Update()
        {
            InputAxisX = Input.GetAxisRaw("Horizontal");
            InputJump = Input.GetButtonDown("Jump");

        }
    }
}
