using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MetroPlatform
{
    public class PlayerAttack : DebuggableMonoBehaviour
    {
        [SerializeField] private float _distance = 0.5f;

        [SerializeField] private ContactFilter2D _contactFilter = new ContactFilter2D();

        [Header("Refs")]
        [SerializeField] private Collider2D _collider;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Animator _animator;
        [SerializeField] private AttackLogicMelee _attackLogicMelee;

        private bool _inputAttack => _playerInput.InputAttack;

        private static readonly int AnimAttack = Animator.StringToHash("AttackPunch");

        private void Awake()
        {
            if (_playerInput == null)
            {
                _playerInput = GetComponentInParent<PlayerInput>();
            }
        }

        private void Update()
        {
            if (_inputAttack)
            {
                _animator.SetTrigger(_attackLogicMelee.AnimAttackParamHash);
            }
        }
        public void Attack()
        {
            _attackLogicMelee.Attack(_collider, _contactFilter, _distance);
        }
    }
}
