using UnityEngine;
using System.Collections;

namespace LoxaRPG.Player.Components
{
    /// <summary>
    /// Мигает красным при уроне, зелёным при хиле.
    /// Подписывается на события PlayerHealth автоматически.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerVisualEffects : MonoBehaviour
    {
        [Header("Цвета")]
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;

        [Header("Настройки")]
        [SerializeField] private float flashDuration = 0.3f;

        [SerializeField] private PlayerHealth playerHealth; // если не назначен — найдём сами

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Coroutine _flashCoroutine;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalColor = _spriteRenderer.color;

            // Если PlayerHealth не назначен — ищем автоматически
            if (playerHealth == null)
            {
                playerHealth = GetComponent<PlayerHealth>();

                if (playerHealth == null)
                {
                    playerHealth = GetComponentInParent<PlayerHealth>();
                }
            }
        }

        private void OnEnable()
        {
            // Подписываемся на события
            if (playerHealth != null)
            {
                playerHealth.OnDamaged.AddListener(OnDamaged);
                playerHealth.OnHealed.AddListener(OnHealed);
            }
        }

        private void OnDisable()
        {
            if (playerHealth != null)
            {
                playerHealth.OnDamaged.RemoveListener(OnDamaged);
                playerHealth.OnHealed.RemoveListener(OnHealed);
            }
        }

        private void OnDamaged(int _)
        {
            StartFlash(damageColor);
        }

        private void OnHealed(int _)
        {
            StartFlash(healColor);
        }

        private void StartFlash(Color color)
        {
            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);

            _flashCoroutine = StartCoroutine(FlashRoutine(color));
        }

        private IEnumerator FlashRoutine(Color color)
        {
            // Три быстрых мигания
            for (int i = 0; i < 3; i++)
            {
                _spriteRenderer.color = color;
                yield return new WaitForSeconds(flashDuration / 6f);
                _spriteRenderer.color = _originalColor;
                yield return new WaitForSeconds(flashDuration / 6f);
            }

            _spriteRenderer.color = _originalColor;
        }
    }
}