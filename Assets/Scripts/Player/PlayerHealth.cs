using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement; // Для перезапуска сцены

public class PlayerHealth : MonoBehaviour
{
    [Header("Здоровье")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("UI")]
    public Slider healthSlider;
    public TMP_Text healthText;
    
    [Header("Эффект урона")]
    public float flashDuration = 0.3f;
    public SpriteRenderer playerSprite;
    
    [Header("Эффект лечения")]
    public Color healColor = Color.green;
    
    [Header("Смерть")]
    public GameObject deathPanel; // Панель смерти (UI)
    public float deathDelay = 2f; // Задержка перед перезапуском
    
    private Color originalColor;
    private Coroutine damageFlashCoroutine;
    private Coroutine healFlashCoroutine;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        
        UpdateHealthText();
        
        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
        }
        
        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
        }
        
        // Скрываем панель смерти
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }
    
    public void TakeDamage(int amount)
    {
        if (isDead) return;
        
        currentHealth -= amount;
        
        // НЕ ДАЁМ ХП УЙТИ В МИНУС
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        
        Debug.Log("Игрок получил урон: " + amount + ", осталось HP: " + currentHealth);
        
        UpdateHealthBar();
        UpdateHealthText();
        ShowDamageEffect();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        Debug.Log("Игрок вылечился на " + amount + ", теперь HP: " + currentHealth);
        
        UpdateHealthBar();
        UpdateHealthText();
        PlayHealEffect();
    }
    
    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            StartCoroutine(SmoothHealthChange(currentHealth));
        
            Image fillImage = healthSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float healthPercent = (float)currentHealth / maxHealth;
            
                if (healthPercent > 0.6f)
                {
                    fillImage.color = Color.green;
                }
                else if (healthPercent > 0.3f)
                {
                    fillImage.color = Color.yellow;
                }
                else
                {
                    fillImage.color = Color.red;
                }
            }
        }
    }

    IEnumerator SmoothHealthChange(float targetHealth)
    {
        float startHealth = healthSlider.value;
        float duration = 0.3f;
        float timer = 0;
    
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            healthSlider.value = Mathf.Lerp(startHealth, targetHealth, t);
            yield return null;
        }
    
        healthSlider.value = targetHealth;
    }
    
    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth + "/" + maxHealth;
        }
    }
    
    void ShowDamageEffect()
    {
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }
        damageFlashCoroutine = StartCoroutine(FlashRed());
    }
    
    IEnumerator FlashRed()
    {
        if (playerSprite == null) yield break;
        
        for (int i = 0; i < 3; i++)
        {
            playerSprite.color = Color.red;
            yield return new WaitForSeconds(flashDuration / 6);
            playerSprite.color = originalColor;
            yield return new WaitForSeconds(flashDuration / 6);
        }
        
        playerSprite.color = originalColor;
    }
    
    public void PlayHealEffect()
    {
        if (healFlashCoroutine != null)
        {
            StopCoroutine(healFlashCoroutine);
        }
        healFlashCoroutine = StartCoroutine(FlashGreen());
    }
    
    IEnumerator FlashGreen()
    {
        if (playerSprite == null) yield break;
        
        for (int i = 0; i < 3; i++)
        {
            playerSprite.color = healColor;
            yield return new WaitForSeconds(0.1f);
            playerSprite.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
        
        playerSprite.color = originalColor;
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("ИГРОК ПОГИБ!");
        
        // Останавливаем движение
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        
        // Отключаем управление
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script != null)
            {
                script.enabled = false;
            }
        }
        
        // Показываем панель смерти
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
        
        // Запускаем перезапуск
        StartCoroutine(RestartAfterDeath());
    }
    
    IEnumerator RestartAfterDeath()
    {
        yield return new WaitForSeconds(deathDelay);
        
        // Перезапускаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public void FullHeal()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        UpdateHealthText();
        PlayHealEffect();
    }
}