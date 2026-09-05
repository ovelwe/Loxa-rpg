using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using LoxaRPG.Enemies.Components;
using LoxaRPG.Systems;
using LoxaRPG.Player.Components;

namespace LoxaRPG.Enemies.Bosses.Grishoid
{
    /// <summary>
    /// Здоровье Гришоида.
    /// С анимацией мигания при уроне и звуком.
    /// </summary>
    public class GrishoidHealth : MonoBehaviour
    {
        [Header("Здоровье")]
        [SerializeField] private int maxHealth = 300;
        [SerializeField] private int moneyReward = 500;

        [Header("Звуки")]
        [SerializeField] private AudioClip hurtSound; // звук получения урона
        [Range(0f, 5f)] [SerializeField] private float hurtSoundVolume = 2f;

        [Header("Анимация урона")]
        [SerializeField] private float flashDuration = 0.1f; // длительность мигания белым

        [Header("Кулдаун звука")]
        [SerializeField] private float hurtSoundCooldown = 0.3f;

        public UnityEvent<int> OnHealthChanged;
        public UnityEvent<int> OnDamaged;
        public UnityEvent OnDeath;

        private int _currentHealth;
        private bool _isDead;
        private float _lastHurtSoundTime;

        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Coroutine _flashCoroutine;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDead => _isDead;

        private void Awake()
        {
            _currentHealth = maxHealth;

            // Получаем SpriteRenderer для анимации урона
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
        }

        public void TakeDamage(float amount)
        {
            if (_isDead) return;

            _currentHealth = Mathf.Max(0, _currentHealth - (int)amount);
            OnHealthChanged?.Invoke(_currentHealth);
            OnDamaged?.Invoke((int)amount);

            // Звук урона
            PlayHurtSound();

            // Анимация мигания белым
            PlayHurtAnimation();

            Debug.Log($"Гришоид получил урон: {amount}, осталось HP: {_currentHealth}");

            if (_currentHealth == 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Проигрывает звук урона с кулдауном.
        /// </summary>
        private void PlayHurtSound()
        {
            if (hurtSound == null) return;

            // Кулдаун, чтобы не спамить звук
            if (Time.time < _lastHurtSoundTime + hurtSoundCooldown) return;
            _lastHurtSoundTime = Time.time;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayHurtSound(hurtSound, transform.position, hurtSoundVolume);
            }
            else
            {
                // Если SoundManager нет — создаём звук напрямую
                var soundObj = new GameObject("GrishoidHurtSound");
                soundObj.transform.position = transform.position;

                var audioSource = soundObj.AddComponent<AudioSource>();
                audioSource.clip = hurtSound;
                audioSource.volume = hurtSoundVolume;
                audioSource.spatialBlend = 0f;
                audioSource.pitch = Random.Range(0.9f, 1.1f);

                audioSource.Play();
                Destroy(soundObj, hurtSound.length);
            }
        }

        /// <summary>
        /// Мигает белым при получении урона.
        /// </summary>
        private void PlayHurtAnimation()
        {
            if (_spriteRenderer == null) return;

            // Останавливаем предыдущую анимацию
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
            }

            _flashCoroutine = StartCoroutine(FlashWhite());
        }

        private IEnumerator FlashWhite()
        {
            if (_spriteRenderer == null) yield break;

            // Мигаем белым
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            _spriteRenderer.color = _originalColor;
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;

            Debug.Log("ГРИШОИД ПОВЕРЖЕН!");
            OnDeath?.Invoke();

            // Даём бабло
            GiveMoney();

            // Сообщаем спавнеру
            ReportDeathToSpawner();

            Destroy(gameObject);
        }

        private void GiveMoney()
        {
            var wallet = FindFirstObjectByType<PlayerWallet>();
            if (wallet != null)
            {
                wallet.Add(moneyReward);
                Debug.Log($"Начислено {moneyReward} монет за босса!");
            }
            else
            {
                Debug.LogError("GrishoidHealth: PlayerWallet не найден!");
            }
        }

        private void ReportDeathToSpawner()
        {
            var reporter = GetComponent<ZombieDeathReporter>();
            if (reporter != null)
            {
                reporter.ReportDeath();
            }
        }
    }
}