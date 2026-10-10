using System.Collections;
using SystemDefender.Scripts.Enemy;
using SystemDefender.Scripts.Services.Abstractions;
using TMPro;
using UnityEngine;

namespace SystemDefender.Scripts.GameProgression
{
    public class GameTimeController : MonoBehaviourSinglton<GameTimeController>
    {
        [SerializeField]
        private TextMeshProUGUI _timerText;
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        private float _timer = 0;
        [SerializeField]
        private TextMeshProUGUI _difficultyModifierText;

        [SerializeField]
        private float _difficultyModifier;

        [SerializeField]
        private float _baseSpawnCapacity;

        [SerializeField]
        private float _difficultyDuration = 60f;
        private Coroutine _currentCoroutine;

        private EnemySystemController _enemySystemController = null;

        private void Start()
        {
            _enemySystemController = EnemySystemController.Instance;

            if(_enemySystemController is null)
            {
                Debug.LogError("Enemy system controller is null");
            }

            startRepeatingCoroutine();
        }

        private void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;

            int minutes = (int)(_timer / 60);
            float seconds = _timer % 60;

            _timerText.text = $"{minutes:D2}:{seconds:00}";

            _scoreText.text = $"{_enemySystemController.getScore()}";

            _difficultyModifierText.text = $"x{_difficultyModifier}";
        }

        void startRepeatingCoroutine()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }

            _enemySystemController.spawnPoolUpdate(_baseSpawnCapacity * _difficultyModifier);

            _currentCoroutine = StartCoroutine(difficultyChanger());
        }

        IEnumerator difficultyChanger()
        {
            yield return new WaitForSeconds(_difficultyDuration);

            _difficultyModifier += _difficultyModifier;

            startRepeatingCoroutine();
        }

        public float getTime()
        {
            return _timer;
        }
    }
}