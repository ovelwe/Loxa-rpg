using UnityEngine;

namespace LoxaRPG.Core
{
    /// <summary>
    /// Ядро игры. Отвечает за глобальное состояние.
    /// Деньги убраны в PlayerWallet, тут только пауза и рестарт.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PauseGame()
        {
            IsPaused = true;
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            IsPaused = false;
            Time.timeScale = 1f;
        }

        public void RestartScene()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}