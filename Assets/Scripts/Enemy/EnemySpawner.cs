using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject grishoidPrefab; // Префаб Гришоида
    [SerializeField] private Transform[] spawnPoints;

    [Header("Waves")]
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int zombiesPerWave = 5;
    [SerializeField] private int zombiesIncreasePerWave = 2;
    [SerializeField] private int wavesBeforeBoss = 10; // Каждые 10 волн — босс

    [Header("Boss")]
    [SerializeField] private float timeBeforeBoss = 3f; // Задержка перед появлением босса
    [SerializeField] private string bossWarningMessage = "ГРИШОИД ПРОБУЖДАЕТСЯ!"; // Сообщение перед боссом

    private int waveNumber;
    private int aliveZombies;
    private bool waveActive;
    private int bossCount = 0; // Количество боссов в текущей волне

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        // Пауза перед следующей волной
        yield return new WaitForSeconds(timeBetweenWaves);

        // Проверяем, нужна ли волна с боссом
        if (waveNumber > 0 && waveNumber % wavesBeforeBoss == 0)
        {
            yield return StartCoroutine(SpawnBossWave());
        }
        else
        {
            SpawnWave();
        }
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
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject zombie = Instantiate(
                zombiePrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            ZombieDeathReporter reporter = zombie.GetComponent<ZombieDeathReporter>();

            if (reporter == null)
                reporter = zombie.AddComponent<ZombieDeathReporter>();

            reporter.Initialize(this);
        }
    }

    private IEnumerator SpawnBossWave()
    {
        // Волна с боссом
        waveNumber++;
        waveActive = true;

        // Вычисляем количество боссов (на 10 волне - 1, на 20 - 2, на 30 - 3 и т.д.)
        bossCount = waveNumber / wavesBeforeBoss;

        Debug.Log($"BOSS WAVE {waveNumber}! ГРИШОИДОВ: {bossCount}");

        // Показываем предупреждение
        ShowBossWarning();

        // Ждём перед появлением босса
        yield return new WaitForSeconds(timeBeforeBoss);

        // Спавним боссов
        SpawnBosses();

        // Спавним немного обычных зомби для поддержки
        int supportZombies = 2 + bossCount; // Чем больше боссов, тем больше поддержки
        aliveZombies = bossCount + supportZombies;

        for (int i = 0; i < supportZombies; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject zombie = Instantiate(
                zombiePrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            ZombieDeathReporter reporter = zombie.GetComponent<ZombieDeathReporter>();

            if (reporter == null)
                reporter = zombie.AddComponent<ZombieDeathReporter>();

            reporter.Initialize(this);
        }
    }

    private void SpawnBosses()
    {
        if (grishoidPrefab == null)
        {
            Debug.LogError("Гришоид префаб не назначен в EnemySpawner!");
            return;
        }

        // Спавним несколько боссов
        for (int i = 0; i < bossCount; i++)
        {
            // Выбираем случайную точку спавна
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Добавляем небольшое случайное смещение, чтобы боссы не спавнились в одной точке
            Vector3 spawnPosition = spawnPoint.position + (Vector3)(Random.insideUnitCircle * 3f);

            // Спавним босса
            GameObject grishoid = Instantiate(
                grishoidPrefab,
                spawnPosition,
                Quaternion.identity
            );

            // Добавляем репортер смерти для босса
            ZombieDeathReporter reporter = grishoid.GetComponent<ZombieDeathReporter>();

            if (reporter == null)
                reporter = grishoid.AddComponent<ZombieDeathReporter>();

            reporter.Initialize(this);

            Debug.Log($"ГРИШОИД {i + 1} из {bossCount} заспавнен!");
        }
    }

    private void ShowBossWarning()
    {
        Debug.LogWarning(bossWarningMessage);

        // Здесь можно показать UI предупреждение
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowBossWarning(bossWarningMessage);
        }
    }

    public void OnZombieDied()
    {
        if (!waveActive)
            return;

        aliveZombies--;

        Debug.Log($"Enemies left: {aliveZombies}");

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

    public int GetBossCount()
    {
        return bossCount;
    }
}