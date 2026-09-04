using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Здоровье")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("UI")]
    public Slider healthSlider; // Слайдер для ХП (сверху экрана)
    public TMP_Text healthText; // Текст с цифрами ХП
    
    [Header("Эффект урона")]
    public float flashDuration = 0.3f; // Длительность красного мигания
    public SpriteRenderer playerSprite; // Спрайт игрока
    
    [Header("Эффект лечения")]
    public Color healColor = Color.green; // Цвет при хиле
    
    private Color originalColor;
    private Coroutine damageFlashCoroutine;
    private Coroutine healFlashCoroutine;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        // Настройка ХП бара
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        
        // Настройка текста
        UpdateHealthText();
        
        // Получаем спрайт игрока
        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
        }
        
        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
        }
    }
    
    public void TakeDamage(int amount)
    {
        if (isDead) return;
        
        currentHealth -= amount;
        Debug.Log("Игрок получил урон: " + amount + ", осталось HP: " + currentHealth);
        
        // Обновляем UI
        UpdateHealthBar();
        UpdateHealthText();
        
        // Показываем эффект урона
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
        
        // Не превышаем максимум
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        Debug.Log("Игрок вылечился на " + amount + ", теперь HP: " + currentHealth);
        
        // Обновляем UI
        UpdateHealthBar();
        UpdateHealthText();
        
        // Показываем эффект лечения
        PlayHealEffect();
    }
    
    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            // Плавно меняем значение
            StartCoroutine(SmoothHealthChange(currentHealth));
        
            // Меняем цвет в зависимости от здоровья
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
        // Останавливаем предыдущую корутину
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }
        damageFlashCoroutine = StartCoroutine(FlashRed());
    }
    
    IEnumerator FlashRed()
    {
        if (playerSprite == null) yield break;
        
        // Мигаем красным несколько раз
        for (int i = 0; i < 3; i++)
        {
            playerSprite.color = Color.red;
            yield return new WaitForSeconds(flashDuration / 6);
            playerSprite.color = originalColor;
            yield return new WaitForSeconds(flashDuration / 6);
        }
        
        // Возвращаем оригинальный цвет
        playerSprite.color = originalColor;
    }
    
    // Публичный метод для эффекта хила
    public void PlayHealEffect()
    {
        // Останавливаем предыдущую корутину
        if (healFlashCoroutine != null)
        {
            StopCoroutine(healFlashCoroutine);
        }
        healFlashCoroutine = StartCoroutine(FlashGreen());
    }
    
    IEnumerator FlashGreen()
    {
        if (playerSprite == null) yield break;
        
        // Мигаем зелёным при хиле
        for (int i = 0; i < 3; i++)
        {
            playerSprite.color = healColor;
            yield return new WaitForSeconds(0.1f);
            playerSprite.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
        
        // Возвращаем оригинальный цвет
        playerSprite.color = originalColor;
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("ИГРОК ПОГИБ!");
        
        // Можно добавить перезапуск сцены или Game Over
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Дополнительные методы
    
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
    
    // Метод для полного восстановления (если нужно)
    public void FullHeal()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        UpdateHealthText();
        PlayHealEffect();
    }
}