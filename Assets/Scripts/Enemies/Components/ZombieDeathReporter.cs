using UnityEngine;
using LoxaRPG.Systems;

namespace LoxaRPG.Enemies.Components
{
    /// <summary>
    /// Сообщает спавнеру о смерти врага.
    /// </summary>
    public class ZombieDeathReporter : MonoBehaviour
    {
        private EnemySpawner _spawner;
        private bool _deathReported;

        public void Initialize(EnemySpawner enemySpawner)
        {
            _spawner = enemySpawner;
        }

        public void ReportDeath()
        {
            if (_deathReported) return;
            _deathReported = true;

            if (_spawner != null)
                _spawner.OnZombieDied();
            else
                Debug.LogWarning("ZombieDeathReporter: Спавнер не назначен!");
        }
    }
}