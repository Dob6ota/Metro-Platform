using UnityEngine;

namespace MetroPlatform
{
    public class AnimEventCatcher : MonoBehaviour
    {
        [SerializeField] PlayerAttack _playerAttack;
        void Start()
        {
        
        }

        public void AnimEvent()
        {
            _playerAttack.Attack();
        }

        public void AnimFinished()
        {
            _playerAttack.Attack();
        }
    }
}
