using UnityEngine;
using System.Collections;
using LoxaRPG.Enemies.Components;
using LoxaRPG.Enemies.Bosses.Grishoid;

namespace LoxaRPG.Systems
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn")]
        [SerializeField] private GameObject zombiePrefab;
        [SerializeField] private GameObject grishoidPrefab;
        [SerializeField] private Transform[] spawnPoints;

        [Header("Waves")]
        [SerializeField] private float timeBetweenWaves = 5f;
        [SerializeField] private int zombiesPerWave = 5;
        [SerializeField] private int zombiesIncreasePerWave = 2;
        [SerializeField] private int wavesBeforeBoss = 10;

        [Header("Boss")]
        [SerializeField] private float timeBeforeBoss = 3f;

        private int _waveNumber;
        private int _aliveEnemies;
        private bool _waveActive;

        private void Start()
        {
            StartCoroutine(StartNextWave());
        }

        private IEnumerator StartNextWave()
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            if (_waveNumber > 0 && _waveNumber % wavesBeforeBoss == 0)
            {
                yield return StartCoroutine(SpawnBossWave());
            }
            else
            {
                SpawnZombieWave();
            }
        }

        private void SpawnZombieWave()
        {
            _waveNumber++;
            _waveActive = true;

            int count = zombiesPerWave + _waveNumber * zombiesIncreasePerWave;
            _aliveEnemies = count;

            Debug.Log($"Волна {_waveNumber}: {count} зомби");

            for (int i = 0; i < count; i++)
            {
                SpawnZombie();
            }
        }

        private IEnumerator SpawnBossWave()
        {
            _waveNumber++;
            _waveActive = true;

            int bossCount = _waveNumber / wavesBeforeBoss;
            Debug.LogWarning($"ВОЛНА {_waveNumber}: {bossCount} ГРИШОИДОВ!");

            yield return new WaitForSeconds(timeBeforeBoss);

            for (int i = 0; i < bossCount; i++)
            {
                SpawnBoss();
            }

            int supportZombies = 2 + bossCount;
            _aliveEnemies = bossCount + supportZombies;

            for (int i = 0; i < supportZombies; i++)
            {
                SpawnZombie();
            }
        }

        private void SpawnZombie()
        {
            if (zombiePrefab == null)
            {
                Debug.LogError("EnemySpawner: Префаб зомби не назначен!");
                return;
            }

            var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var zombie = Instantiate(zombiePrefab, point.position, Quaternion.identity);

            var reporter = zombie.GetComponent<ZombieDeathReporter>();
            if (reporter == null)
                reporter = zombie.AddComponent<ZombieDeathReporter>();

            reporter.Initialize(this);
        }

        private void SpawnBoss()
        {
            if (grishoidPrefab == null)
            {
                Debug.LogError("EnemySpawner: Префаб Гришоида не назначен!");
                return;
            }

            var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var offset = (Vector3)(Random.insideUnitCircle * 3f);

            var boss = Instantiate(grishoidPrefab, point.position + offset, Quaternion.identity);

            var reporter = boss.GetComponent<ZombieDeathReporter>();
            if (reporter == null)
                reporter = boss.AddComponent<ZombieDeathReporter>();

            reporter.Initialize(this);
        }

        public void OnZombieDied()
        {
            if (!_waveActive) return;

            _aliveEnemies--;
            Debug.Log($"Врагов осталось: {_aliveEnemies}");

            if (_aliveEnemies <= 0)
            {
                _aliveEnemies = 0;
                _waveActive = false;
                Debug.Log($"Волна {_waveNumber} завершена!");
                StartCoroutine(StartNextWave());
            }
        }
    }
}