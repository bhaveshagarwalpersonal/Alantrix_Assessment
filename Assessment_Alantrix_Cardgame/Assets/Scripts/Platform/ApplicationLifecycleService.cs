using UnityEngine;
using EchoGrid.Core;

namespace EchoGrid.Platform
{
    public sealed class ApplicationLifecycleService :
        MonoBehaviour
    {
        [SerializeField]
        private GameSessionController
            gameSession;

        [SerializeField]
        private GameObject pausePanel;

        private void OnApplicationPause(
            bool pauseStatus)
        {
            if (
                pauseStatus)
            {
                gameSession.SaveGame();
            }
        }

        private void OnApplicationFocus(
            bool hasFocus)
        {
            if (
                !hasFocus)
            {
              gameSession.SaveGame();
            }
        }

        private void Update()
        {
            if (
                Application.platform ==
                RuntimePlatform.Android
                &&
                Input.GetKeyDown(
                    KeyCode.Escape))
            {
                gameSession.SaveGame();

                pausePanel
                    ?.SetActive(
                        true);
            }
        }
        public void PauseGame() 
        {
            gameSession.SaveGame();

            pausePanel
                ?.SetActive(
                    true);
        }
    }

    
}