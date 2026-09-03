using UnityEngine;
using System;

public class ZombieHealth : MonoBehaviour
{
    public float maxHealth = 30f;
    private float currentHealth;
    public int moneyReward = 10;
    
    // Событие смерти
    public event Action<GameObject> OnZombieDeath;
    
    // World Space HP бар
    public WorldHealthBar worldHealthBar;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Обновляем ХП бар при старте
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Зомби получил урон: " + amount + ", осталось HP: " + currentHealth);
        
        // Показываем урон (если есть DamagePopup)
        if (DamagePopup.Instance != null)
        {
            DamagePopup.Instance.ShowDamage(transform.position + Vector3.up * 0.5f, amount);
        }
        
        // Обновляем ХП бар
        UpdateHealthBar();
        
        // Мигаем красным при уроне
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            Invoke("ResetColor", 0.1f);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (worldHealthBar != null)
        {
            worldHealthBar.SetHealth(currentHealth, maxHealth);
        }
    }

    void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    void Die()
    {
        Debug.Log("Зомби сдох!");
        
        ZombieDeathReporter reporter =
            GetComponent<ZombieDeathReporter>();

        if (reporter != null)
            reporter.ReportDeath();
        
        // Вызываем событие смерти
        if (OnZombieDeath != null)
        {
            OnZombieDeath(gameObject);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(moneyReward);
        }
        
        Destroy(gameObject);
    }
}