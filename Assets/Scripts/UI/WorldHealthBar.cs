using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace LoxaRPG.UI
{
    /// <summary>
    /// ХП бар над зомби.
    /// Плавно уменьшается и меняет цвет: зелёный → жёлтый → красный.
    /// </summary>
    public class WorldHealthBar : MonoBehaviour
    {
        [Header("Цвета")]
        [SerializeField] private Color highColor = Color.green;
        [SerializeField] private Color mediumColor = Color.yellow;
        [SerializeField] private Color lowColor = Color.red;

        [Header("Настройки анимации")]
        [SerializeField] private float smoothSpeed = 5f; // скорость плавного изменения

        [Header("UI")]
        [SerializeField] private Image fillImage; // Перетащи СЮДА Fill из Canvas

        private Coroutine _smoothCoroutine;
        private float _targetFillAmount = 1f;

        public void SetHealth(float currentHealth, float maxHealth)
        {
            if (fillImage == null)
            {
                Debug.LogError("WorldHealthBar: fillImage не назначен!");
                return;
            }

            float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
            _targetFillAmount = healthPercent;

            // Запускаем плавное изменение
            if (_smoothCoroutine != null)
            {
                StopCoroutine(_smoothCoroutine);
            }
            _smoothCoroutine = StartCoroutine(SmoothHealthChange());
        }

        private IEnumerator SmoothHealthChange()
        {
            while (Mathf.Abs(fillImage.fillAmount - _targetFillAmount) > 0.01f)
            {
                // Плавно меняем fillAmount
                fillImage.fillAmount = Mathf.Lerp(
                    fillImage.fillAmount,
                    _targetFillAmount,
                    smoothSpeed * Time.deltaTime
                );

                // Обновляем цвет
                UpdateColor(fillImage.fillAmount);

                yield return null;
            }

            // Финально ставим точное значение
            fillImage.fillAmount = _targetFillAmount;
            UpdateColor(fillImage.fillAmount);
        }

        private void UpdateColor(float healthPercent)
        {
            if (healthPercent > 0.6f)
            {
                fillImage.color = highColor; // зелёный
            }
            else if (healthPercent > 0.3f)
            {
                fillImage.color = mediumColor; // жёлтый
            }
            else
            {
                fillImage.color = lowColor; // красный
            }
        }
    }
}