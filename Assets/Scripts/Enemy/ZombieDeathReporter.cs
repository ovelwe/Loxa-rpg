using UnityEngine;

public class ZombieDeathReporter : MonoBehaviour
{
    private EnemySpawner spawner;
    private bool deathReported;

    public void Initialize(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
    }

    public void ReportDeath()
    {
        if (deathReported)
            return;

        deathReported = true;

        if (spawner != null)
            spawner.OnZombieDied();
    }
}