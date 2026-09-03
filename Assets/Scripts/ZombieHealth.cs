using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ZombieHealth : MonoBehaviour
{
    public float maxHealth = 30f;
    private float currentHealth;
    public int moneyReward = 10;
    
    [Header("Звуки")]
    public AudioClip deathSound; // Звук смерти
    
    [Header("Громкость (можно больше 1!)")]
    [Range(0f, 5f)] public float deathSoundVolume = 2f; // Громкость смерти
    
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

    // Проигрываем звук смерти с настройкой громкости
    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            // Создаём временный объект для ГРОМКОГО звука
            GameObject soundObject = new GameObject("LoudDeathSound");
            soundObject.transform.position = transform.position;
            
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = deathSound;
            
            // НАСТРОЙКА ГРОМКОСТИ
            audioSource.volume = deathSoundVolume; // Может быть 2, 3, 5!
            audioSource.spatialBlend = 0f; // 0 = 2D звук (слышно везде одинаково)
            audioSource.bypassEffects = true;
            audioSource.bypassListenerEffects = true;
            audioSource.bypassReverbZones = true;
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Разброс тона
            
            audioSource.Play();
            
            // Уничтожаем после проигрывания
            Destroy(soundObject, deathSound.length);
        }
    }
    
    void Die()
    {
        Debug.Log("Зомби сдох!");
        
        ZombieDeathReporter reporter =
            GetComponent<ZombieDeathReporter>();

        if (reporter != null)
            reporter.ReportDeath();
        
        // Проигрываем звук смерти
        PlayDeathSound();
        
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