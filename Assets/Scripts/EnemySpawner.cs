using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;
    public int zombiesPerWave = 5;
    private float timer;
    private int waveNumber = 0;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeBetweenWaves)
        {
            timer = 0;
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        waveNumber++;
        int count = zombiesPerWave + waveNumber * 2; // усложняем
        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(zombiePrefab, point.position, Quaternion.identity);
        }
    }
}