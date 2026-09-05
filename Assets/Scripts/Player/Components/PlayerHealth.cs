using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace LoxaRPG.Player.Components
{
    /// <summary>
    /// Здоровье игрока.
    /// С миганием при уроне и звуком.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Здоровье")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;

        [Header("Звук урона")]
        [SerializeField] private AudioClip hurtSound;
        [Range(0f, 5f)] [SerializeField] private float hurtSoundVolume = 2f;

        public UnityEvent<int> OnHealthChanged;
        public UnityEvent<int> OnDamaged;
        public UnityEvent<int> OnHealed;
        public UnityEvent OnDeath;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Coroutine _flashCoroutine;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDead { get; private set; }

        private void Awake()
        {
            currentHealth = maxHealth;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth);
            OnDamaged?.Invoke(amount);

            // МИГАЕМ КРАСНЫМ
            FlashRed();

            // ЗВУК УРОНА
            PlayHurtSound();

            Debug.Log($"Игрок получил урон: {amount}, HP: {currentHealth}");

            if (currentHealth == 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            if (IsDead) return;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth);
            OnHealed?.Invoke(amount);

            // МИГАЕМ ЗЕЛЁНЫМ
            FlashGreen();
        }

        private void PlayHurtSound()
        {
            if (hurtSound == null) return;

            // СОЗДАЁМ ЗВУК НАПРЯМУЮ, БЕЗ SoundManager!
            var soundObj = new GameObject("PlayerHurtSound");
            soundObj.transform.position = transform.position;

            var audioSource = soundObj.AddComponent<AudioSource>();
            audioSource.clip = hurtSound;
            audioSource.volume = hurtSoundVolume;
            audioSource.spatialBlend = 0f; // 2D звук
            audioSource.pitch = Random.Range(0.9f, 1.1f);

            audioSource.Play();
            Destroy(soundObj, hurtSound.length);
        }

        private void FlashRed()
        {
            if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(Flash(Color.red));
        }

        private void FlashGreen()
        {
            if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(Flash(Color.green));
        }

        private IEnumerator Flash(Color color)
        {
            if (_spriteRenderer == null) yield break;

            for (int i = 0; i < 3; i++)
            {
                _spriteRenderer.color = color;
                yield return new WaitForSeconds(0.1f);
                _spriteRenderer.color = _originalColor;
                yield return new WaitForSeconds(0.1f);
            }

            _spriteRenderer.color = _originalColor;
        }

        private void Die()
        {
            if (IsDead) return;
            IsDead = true;
            OnDeath?.Invoke();
        }
    }
}