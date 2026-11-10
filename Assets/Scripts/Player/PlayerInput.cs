using UnityEngine;

namespace MetroPlatform
{
    public class PlayerInput : MonoBehaviour
    {
        public float InputAxisX;
        public bool InputJump;
        public bool InputAttack;
        void Start()
        {
        
        }

        void Update()
        {
            InputAxisX = Input.GetAxisRaw("Horizontal");
            InputJump = Input.GetButtonDown("Jump");
            InputAttack = Input.GetMouseButtonDown(0);
        }
    }
}
