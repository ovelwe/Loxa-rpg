using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LoxaRPG.Player.Components;

namespace LoxaRPG.UI
{
    /// <summary>
    /// Показывает хп игрока в UI.
    /// Слушает события PlayerHealth и обновляет слайдер с текстом.
    /// </summary>
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private GameObject deathPanel; // панель смерти

        private void OnEnable()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged.AddListener(UpdateHealth);
                playerHealth.OnDeath.AddListener(ShowDeathScreen);
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged.RemoveListener(UpdateHealth);
                playerHealth.OnDeath.RemoveListener(ShowDeathScreen);
            }
        }

        private void Start()
        {
            if (healthSlider != null && playerHealth != null)
            {
                healthSlider.maxValue = playerHealth.MaxHealth;
                healthSlider.value = playerHealth.CurrentHealth;
            }
        }

        private void UpdateHealth(int currentHealth)
        {
            if (healthSlider != null)
                healthSlider.value = currentHealth;

            if (healthText != null)
                healthText.text = $"{currentHealth}/{playerHealth.MaxHealth}";
        }

        private void ShowDeathScreen()
        {
            Debug.Log("Игрок умер!");
            
            if (deathPanel != null)
                deathPanel.SetActive(true);

            // Останавливаем игру
            Time.timeScale = 0f;
        }
    }
}