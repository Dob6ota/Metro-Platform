using UnityEngine;

namespace MetroPlatform
{
    public class ProjectileSpawner : DebuggableMonoBehaviour
    {
        [SerializeField] private GameObject _projPrefab;
        [SerializeField] private float _projSpeed = 6.0f;
        [SerializeField] private Vector3 _projDirection = Vector3.right;
        [SerializeField] private Transform _attackT;
        [SerializeField] private Collider2D _collider;

        private void Awake()
        {
            if (_attackT == null)
            {
                _attackT = GetComponent<Transform>();
            }
        }

        [ContextMenu("Spawn")]

        public void Spawn()
        {
            var spawnedInst = Instantiate(_projPrefab, _attackT.position, Quaternion.identity);

            if (spawnedInst.TryGetComponent<Projectile>(out Projectile projectile))
            {
                projectile.Initialize(_projSpeed, _projDirection, _collider);
            }
        }
    }
}
