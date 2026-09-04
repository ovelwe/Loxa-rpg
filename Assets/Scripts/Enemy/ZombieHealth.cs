using UnityEngine;
using System;

public class ZombieHealth : MonoBehaviour
{
    public float maxHealth = 30f;
    private float currentHealth;
    public int moneyReward = 10;
    
    [Header("Звуки")]
    public AudioClip deathSound;
    public AudioClip hurtSound;
    
    [Header("Громкость")]
    [Range(0f, 5f)] public float deathSoundVolume = 2f;
    [Range(0f, 5f)] public float hurtSoundVolume = 2f;
    
    public event Action<GameObject> OnZombieDeath;
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
        
        UpdateHealthBar();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Зомби получил урон: " + amount + ", осталось HP: " + currentHealth);
        
        // Используем SoundManager для звука урона (с кулдауном)
        PlayHurtSound();
        
        if (DamagePopup.Instance != null)
        {
            DamagePopup.Instance.ShowDamage(transform.position + Vector3.up * 0.5f, amount);
        }
        
        UpdateHealthBar();
        
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
    
    void PlayHurtSound()
    {
        if (SoundManager.Instance != null)
        {
            // Используем специальный метод с кулдауном
            SoundManager.Instance.PlayHurtSound(hurtSound, transform.position, hurtSoundVolume);
        }
        else
        {
            // Если SoundManager нет — проигрываем напрямую
            if (hurtSound != null)
            {
                AudioSource.PlayClipAtPoint(hurtSound, transform.position);
            }
        }
    }

    public void PlayDeathSound()
    {
        if (SoundManager.Instance != null)
        {
            // Звук смерти без кулдауна
            SoundManager.Instance.PlaySoundNoCooldown(deathSound, transform.position, deathSoundVolume);
        }
        else
        {
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }
        }
    }
    
    void Die()
    {
        Debug.Log("Зомби сдох!");
        
        ZombieDeathReporter reporter = GetComponent<ZombieDeathReporter>();
        if (reporter != null)
            reporter.ReportDeath();
        
        PlayDeathSound();
        
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