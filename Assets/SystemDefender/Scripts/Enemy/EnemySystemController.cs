using System.Collections.Generic;
using SystemDefender.Scripts.GameProgression;
using SystemDefender.Scripts.Ground;
using SystemDefender.Scripts.Services.Abstractions;
using UnityEngine;

namespace SystemDefender.Scripts.Enemy
{
    public class EnemySystemController : MonoBehaviourSinglton<EnemySystemController>
    {
        [SerializeField]
        private List<EnemyBehaviorController> _enemyPrefabs;
        private List<EnemyBehaviorController> _enemies = new List<EnemyBehaviorController>();

        private float _currentSpawnPool = 0;
        private float _currentSpawnPoolInUse = 0;

        private GameTimeController _gameTimeController = null;
        private GroundController _groundController = null;

        [SerializeField]
        private float _minSpawnDistance = 100;
        [SerializeField]
        private float _maxSpawnDistance = 200;
        [SerializeField]
        private float _destroyDistance = 300;

        private float _score = 0;

        private void Start()
        {
            _gameTimeController = GameTimeController.Instance;
            _groundController = GroundController.Instance;

            if (_gameTimeController is null)
            {
                Debug.LogError("Game time controller is null");
            }

            if (_groundController is null)
            {
                Debug.LogError("Ground controller is null");
            }
        }

        private void FixedUpdate()
        {
            foreach (EnemyBehaviorController enemy in _enemyPrefabs)
            {
                EnemyParameters enemyParameters = enemy.getEnemyParameters();

                if (_currentSpawnPool - _currentSpawnPoolInUse >= enemyParameters.SpawnCost
                    && Random.value >= enemyParameters.SpawnChance)
                {
                    spawnEnemy(enemy);
                }
            }
        }

        private void spawnEnemy(EnemyBehaviorController enemy)
        {
            Vector3? spawnPosition = _groundController.getSpawnLocation(_minSpawnDistance, _maxSpawnDistance);

            if (!spawnPosition.HasValue)
            {
                return;
            }

            _currentSpawnPoolInUse += enemy.getEnemyParameters().SpawnCost;

            GameObject spawnedEnemy = Instantiate(enemy.gameObject, spawnPosition.Value, Quaternion.identity, transform);

            _enemies.Add(spawnedEnemy.GetComponent<EnemyBehaviorController>());
        }

        public void destroyEnemy(EnemyBehaviorController __enemy)
        {
            _enemies.Remove(__enemy);
            _currentSpawnPoolInUse -= __enemy.getEnemyParameters().SpawnCost;
            Destroy(__enemy.gameObject);
        }

        public void riseScore(EnemyBehaviorController __enemy)
        {
            _score += __enemy.getEnemyParameters().KillValue;
        }

        public void spawnPoolUpdate(float __currentSpawnPool)
        {
            _currentSpawnPool = __currentSpawnPool;
        }

        public float getMaxDistance()
        {
            return _destroyDistance;
        }

        public float getScore()
        {
            return _score;
        }
    }
}