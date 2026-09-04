using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Waves")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int zombiesPerWave = 5;
    [SerializeField] private int zombiesIncreasePerWave = 2;

    private int waveNumber;
    private int aliveZombies;

    private bool waveActive;

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        // Пауза перед следующей волной
        yield return new WaitForSeconds(timeBetweenWaves);

        SpawnWave();
    }

    private void SpawnWave()
    {
        waveNumber++;
        waveActive = true;

        int count = zombiesPerWave + waveNumber * zombiesIncreasePerWave;

        aliveZombies = count;

        Debug.Log($"Wave {waveNumber} started. Zombies: {count}");

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint =
                spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject zombie = Instantiate(
                zombiePrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            // Передаём зомби ссылку на его Spawner
            ZombieDeathReporter reporter =
                zombie.GetComponent<ZombieDeathReporter>();

            if (reporter == null)
                reporter = zombie.AddComponent<ZombieDeathReporter>();

            reporter.Initialize(this);
        }
    }

    public void OnZombieDied()
    {
        if (!waveActive)
            return;

        aliveZombies--;

        Debug.Log($"Zombies left: {aliveZombies}");

        if (aliveZombies <= 0)
        {
            aliveZombies = 0;
            waveActive = false;

            Debug.Log($"Wave {waveNumber} completed!");

            StartCoroutine(StartNextWave());
        }
    }

    public int GetWaveNumber()
    {
        return waveNumber;
    }

    public int GetAliveZombies()
    {
        return aliveZombies;
    }
}