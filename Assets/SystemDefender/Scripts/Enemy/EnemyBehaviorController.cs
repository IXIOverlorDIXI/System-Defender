using SystemDefender.Scripts.Player;
using UnityEngine;

namespace SystemDefender.Scripts.Enemy
{
    public class EnemyBehaviorController : MonoBehaviour
    {
        [SerializeField]
        private EnemyParameters _enemyParameters;

        private PlayerController _playerController = null;
        private EnemySystemController _enemySystemController = null;

        private bool _isAlive = true;
        private Camera _camera = null;
        private RectTransform _rectTransform = null;

        private bool _isViewedPreviously = false;

        private void Start()
        {
            _playerController = PlayerController.Instance;
            _enemySystemController = EnemySystemController.Instance;

            if (_playerController is null)
            {
                Debug.LogError("Player controller does not exist");
            }

            if (_enemySystemController is null)
            {
                Debug.LogError("Enemy system controller does not exist");
            }

            _camera = Camera.main;
            _rectTransform = GetComponentInChildren<RectTransform>();
        }

        private void FixedUpdate()
        {
            float distanceToTarget = Vector3.Distance(transform.position, _playerController.transform.position);

            if (_isAlive && distanceToTarget >= _enemySystemController.getMaxDistance())
            {
                _enemySystemController.destroyEnemy(this);
                _isAlive = false;

                return;
            }

            Vector3 direction = _playerController.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            transform.position += direction * _enemyParameters.Speed * Time.fixedDeltaTime;
        }

        private bool getIsVisible()
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);

            Bounds bounds = CalculateWorldBounds(_rectTransform);

            bool isVisible = GeometryUtility.TestPlanesAABB(planes, bounds);

            return isVisible;
        }

        private void LateUpdate()
        {
            bool isVisible = getIsVisible();

            if (_isViewedPreviously && !isVisible)
            {
                _playerController.enemyOutOfView(_enemyParameters.GpuDamage);
                _isViewedPreviously = false;
            }
            if (!_isViewedPreviously && isVisible)
            {
                _playerController.enemyInView(_enemyParameters.GpuDamage);
                _isViewedPreviously = true;
            }
        }

        private Bounds CalculateWorldBounds(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector3 center = Vector3.zero;
            foreach (Vector3 corner in corners)
            {
                center += corner;
            }
            center /= 4;

            Bounds bounds = new Bounds(center, Vector3.zero);
            foreach (Vector3 corner in corners)
            {
                bounds.Encapsulate(corner);
            }

            return bounds;
        }

        public EnemyParameters getEnemyParameters()
        {
            return _enemyParameters;
        }

        public void death()
        {
            if (_isViewedPreviously)
            {
                _playerController.enemyOutOfView(_enemyParameters.GpuDamage);
            }
            _isAlive = false;

            _enemySystemController.riseScore(this);
            _enemySystemController.destroyEnemy(this);
        }

        public void damage()
        {
            _playerController.systemDamage(_enemyParameters.Damage);

            death();
        }
    }
}
