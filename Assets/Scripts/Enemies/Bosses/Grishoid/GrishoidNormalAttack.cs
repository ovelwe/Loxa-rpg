using UnityEngine;
using System.Collections;
using LoxaRPG.Player.Components;

namespace LoxaRPG.Enemies.Bosses.Grishoid
{
    /// <summary>
    /// Обычная атака Гришоида.
    /// Замах, удар, возврат. С звуком и настраиваемым питчем.
    /// </summary>
    public class GrishoidNormalAttack : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private float attackRange = 2.5f;
        [SerializeField] private int attackDamage = 15;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private float windupDuration = 0.2f;
        [SerializeField] private float returnDuration = 0.2f;

        [Header("Звук")]
        [SerializeField] private AudioClip attackSound;
        [Range(0f, 5f)] [SerializeField] private float soundVolume = 2f;
        [Range(0.1f, 3f)] [SerializeField] private float soundPitch = 1f; // НАСТРОЙКА ПИТЧА!

        private Transform _player;
        private float _lastAttackTime;
        private bool _isAttacking;

        private void Start()
        {
            FindPlayer();
            InvokeRepeating(nameof(FindPlayer), 0f, 2f);
        }

        private void FindPlayer()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }

        public bool CanAttack()
        {
            return !_isAttacking && Time.time >= _lastAttackTime + attackCooldown;
        }

        public void StartAttack()
        {
            if (!CanAttack()) return;
            StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            _isAttacking = true;
            _lastAttackTime = Time.time;

            var originalPos = transform.position;
            var windupPos = originalPos + Vector3.up * 0.5f;

            float windupTimer = 0;
            while (windupTimer < windupDuration)
            {
                windupTimer += Time.deltaTime;
                float t = windupTimer / windupDuration;
                transform.position = Vector3.Lerp(originalPos, windupPos, t);
                yield return null;
            }

            // УДАР! ЗВУК С ПИТЧЕМ!
            PlaySoundDirect(attackSound);

            if (_player != null)
            {
                float distance = Vector2.Distance(transform.position, _player.position);
                if (distance <= attackRange)
                {
                    if (_player.TryGetComponent<PlayerHealth>(out var health))
                    {
                        health.TakeDamage(attackDamage);
                    }
                }
            }

            float returnTimer = 0;
            while (returnTimer < returnDuration)
            {
                returnTimer += Time.deltaTime;
                float t = returnTimer / returnDuration;
                transform.position = Vector3.Lerp(windupPos, originalPos, t);
                yield return null;
            }

            _isAttacking = false;
        }

        private void PlaySoundDirect(AudioClip clip)
        {
            if (clip == null) return;

            var soundObj = new GameObject("GrishoidAttackSound");
            soundObj.transform.position = transform.position;

            var audioSource = soundObj.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = soundVolume;
            audioSource.pitch = soundPitch; // ИСПОЛЬЗУЕМ ПИТЧ!
            audioSource.spatialBlend = 0f;

            audioSource.Play();
            Destroy(soundObj, clip.length);
        }
    }
}