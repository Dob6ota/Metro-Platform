using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace MetroPlatform
{
    [CreateAssetMenu(fileName = "AttackLogicMelee new", menuName = "Scriptable Objects/AttackLogicMelee")]
    public class AttackLogicMelee : ScriptableObject
    {
        [SerializeField] public float Damage = 5f;

        [HideInInspector] public int AnimAttackParamHash;
        [SerializeField] protected string AnimAttackName;

        private void OnValidate()
        {
            AnimAttackParamHash = Animator.StringToHash(AnimAttackName);
        }

        public void Attack(Collider2D collider, ContactFilter2D contactFilter, float distance)
        {
            Vector2 castDir = collider.transform.TransformDirection(Vector2.right);
            List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();

            collider.Cast(castDir, contactFilter, raycastHits, distance);

            foreach (var hit in raycastHits)
            {
                Debug.Log($"PlayerAttack.Attack hit: {hit.transform.name}");
                if (hit.transform.TryGetComponent<IDamageable>(out IDamageable dmgTarget))
                {
                    dmgTarget.ReceiveDamage(Damage);
                }
            }
        }
    }
}
