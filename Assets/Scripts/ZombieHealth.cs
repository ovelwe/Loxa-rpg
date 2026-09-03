using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float maxHealth = 30f;
    private float currentHealth;
    public int moneyReward = 10; // валюта за убийство

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Даём игроку валюту (для доната)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(moneyReward);
        }
        else
        {
            Debug.LogError("GameManager.Instance is null! Создай GameManager на сцене!");
        }
        
        Destroy(gameObject);
    }
}