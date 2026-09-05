using UnityEngine;
using UnityEngine.Events;
using LoxaRPG.Systems;
using LoxaRPG.Player.Components;
using LoxaRPG.UI;

namespace LoxaRPG.Enemies.Components
{
    /// <summary>
    /// Здоровье зомби.
    /// При смерти сообщает спавнеру через ZombieDeathReporter.
    /// </summary>
    public class ZombieHealth : MonoBehaviour
    {
        [Header("Здоровье")]
        [SerializeField] private int maxHealth = 30;
        [SerializeField] private int moneyReward = 10;

        [Header("Звуки")]
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip hurtSound;

        [Header("Громкость")]
        [Range(0f, 5f)] [SerializeField] private float deathSoundVolume = 2f;
        [Range(0f, 5f)] [SerializeField] private float hurtSoundVolume = 2f;

        public UnityEvent<int> OnHealthChanged;
        public UnityEvent<int> OnDamaged;
        public UnityEvent OnDeath;

        private int _currentHealth;
        private bool _isDead;

        [SerializeField] private WorldHealthBar worldHealthBar;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (_isDead) return;

            _currentHealth = Mathf.Max(0, _currentHealth - (int)amount);
            OnHealthChanged?.Invoke(_currentHealth);
            OnDamaged?.Invoke((int)amount);

            PlayHurtSound();

            if (worldHealthBar != null)
                worldHealthBar.SetHealth(_currentHealth, maxHealth);

            if (_currentHealth == 0)
            {
                Die();
            }
        }

        private void PlayHurtSound()
        {
            if (hurtSound == null) return;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayHurtSound(hurtSound, transform.position, hurtSoundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(hurtSound, transform.position);
            }
        }

        private void PlayDeathSound()
        {
            if (deathSound == null) return;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(deathSound, transform.position, deathSoundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;

            Debug.Log("Зомби сдох!");

            PlayDeathSound();
            OnDeath?.Invoke();

            // ДАЁМ БАБЛО
            GiveMoney();

            // СООБЩАЕМ СПАВНЕРУ О СМЕРТИ!
            ReportDeathToSpawner();

            Destroy(gameObject);
        }

        private void GiveMoney()
        {
            var wallet = FindFirstObjectByType<PlayerWallet>();
            if (wallet != null)
            {
                wallet.Add(moneyReward);
                Debug.Log($"Начислено {moneyReward} монет");
            }
        }

        private void ReportDeathToSpawner()
        {
            var reporter = GetComponent<ZombieDeathReporter>();
            if (reporter != null)
            {
                reporter.ReportDeath();
            }
            else
            {
                Debug.LogWarning("ZombieDeathReporter не найден на зомби!");
            }
        }
    }
}