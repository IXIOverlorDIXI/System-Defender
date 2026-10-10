using System.Linq;
using System.Threading.Tasks;
using SystemDefender.Scripts.Enemy;
using SystemDefender.Scripts.GameProgression;
using SystemDefender.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemDefender.Scripts.UI
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _timer;
        [SerializeField]
        private TextMeshProUGUI _score;

        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip _gameOverSound;
     
        private void Start()
        {
            EnemySystemController.Instance.gameObject.SetActive(false);
            Camera.main.transform.SetParent(transform);
            PlayerController.Instance.gameObject.SetActive(false);

            FindObjectsByType<EnemyBehaviorController>(FindObjectsSortMode.None)
                .ToList()
                .ForEach(x => x.gameObject.SetActive(false));

            float timer = GameTimeController.Instance.getTime();

            int minutes = (int)(timer / 60);
            float seconds = timer % 60;

            _timer.text = $"You have survived: {minutes:D2}:{seconds:00}";
            _score.text = $"Your score: {EnemySystemController.Instance.getScore()}";

            _audioSource.clip = _gameOverSound;
            _audioSource.loop = false;
            _audioSource.Play();
        }

        public async void onMainMenu()
        {
            await SceneManager.LoadSceneAsync(0);
        }
    }
}