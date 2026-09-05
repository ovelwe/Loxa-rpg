using UnityEngine;
using System.Collections;

namespace LoxaRPG.Player.Components
{
    /// <summary>
    /// Стан игрока.
    /// Отключает все скрипты на время стана, чтобы игрок не мог двигаться.
    /// </summary>
    public class StunEffect : MonoBehaviour
    {
        [SerializeField] private GameObject stunVisual;

        private bool _isStunned;
        private MonoBehaviour[] _playerScripts;

        private void Start()
        {
            // Собираем все скрипты игрока, чтобы отключать их при стане
            _playerScripts = GetComponents<MonoBehaviour>();
        }

        public void StunPlayer(float duration)
        {
            if (_isStunned) return;
            StartCoroutine(StunRoutine(duration));
        }

        private IEnumerator StunRoutine(float duration)
        {
            _isStunned = true;

            // Отключаем все скрипты, кроме этого и PlayerHealth
            foreach (var script in _playerScripts)
            {
                if (script != null && script != this && script.GetType() != typeof(PlayerHealth))
                {
                    script.enabled = false;
                }
            }

            // Показываем визуал стана
            if (stunVisual != null)
                stunVisual.SetActive(true);

            yield return new WaitForSeconds(duration);

            // Включаем скрипты обратно
            foreach (var script in _playerScripts)
            {
                if (script != null && script != this && script.GetType() != typeof(PlayerHealth))
                {
                    script.enabled = true;
                }
            }

            if (stunVisual != null)
                stunVisual.SetActive(false);

            _isStunned = false;
        }
    }
}