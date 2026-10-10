using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemDefender.Scripts.UI
{
    public class MainUIButtonsHandler : MonoBehaviour
    {
        public async void NewGameOnClick()
        {
            await SceneManager.LoadSceneAsync(1);
        }

        public void QuitOnClick()
        {
            Application.Quit();
        }
    }
}
