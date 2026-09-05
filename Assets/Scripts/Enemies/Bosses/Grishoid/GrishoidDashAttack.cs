using UnityEngine;
using System.Collections;
using LoxaRPG.Player.Components;

namespace LoxaRPG.Enemies.Bosses.Grishoid
{
    /// <summary>
    /// Атака вылетом Гришоида.
    /// Звуки с настраиваемым питчем.
    /// </summary>
    public class GrishoidDashAttack : MonoBehaviour
    {
        [Header("Настройки вылета")]
        [SerializeField] private float dashDistance = 6f;
        [SerializeField] private float dashDuration = 0.3f;
        [SerializeField] private float dashCooldown = 3f;
        [SerializeField] private int dashDamage = 20;

        [Header("Настройка попадания")]
        [SerializeField] private float hitRadius = 2f;

        [Header("Звук вылета")]
        [SerializeField] private AudioClip dashSound;
        [Range(0f, 5f)] [SerializeField] private float dashVolume = 2f;
        [Range(0.1f, 3f)] [SerializeField] private float dashPitch = 1f; // ПИТЧ ВЫЛЕТА

        [Header("Звук удара")]
        [SerializeField] private AudioClip impactSound;
        [Range(0f, 5f)] [SerializeField] private float impactVolume = 2f;
        [Range(0.1f, 3f)] [SerializeField] private float impactPitch = 1f; // ПИТЧ УДАРА

        private Transform _player;
        private float _lastDashTime;
        private bool _isDashing;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }

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

        public bool CanDash()
        {
            return !_isDashing && Time.time >= _lastDashTime + dashCooldown;
        }

        public void StartDash()
        {
            if (!CanDash()) return;
            StartCoroutine(DashRoutine());
        }

        private IEnumerator DashRoutine()
        {
            _isDashing = true;
            _lastDashTime = Time.time;

            var originalPos = transform.position;
            var directionToPlayer = (_player.position - transform.position).normalized;
            var targetPos = originalPos + directionToPlayer * dashDistance;

            float actualDistance = Vector2.Distance(originalPos, _player.position);
            if (actualDistance < dashDistance)
            {
                targetPos = _player.position;
            }

            // ЗВУК ВЫЛЕТА С ПИТЧЕМ!
            PlaySoundDirect(dashSound, dashVolume, dashPitch);

            if (_spriteRenderer != null)
                _spriteRenderer.color = Color.white;

            float flyTimer = 0;
            while (flyTimer < dashDuration)
            {
                flyTimer += Time.deltaTime;
                float t = flyTimer / dashDuration;
                float easedT = t * t * (3f - 2f * t);
                transform.position = Vector3.Lerp(originalPos, targetPos, easedT);
                yield return null;
            }

            if (_spriteRenderer != null)
                _spriteRenderer.color = _originalColor;

            float distanceAfterDash = Vector2.Distance(transform.position, _player.position);
            if (distanceAfterDash <= hitRadius)
            {
                if (_player.TryGetComponent<PlayerHealth>(out var health))
                {
                    health.TakeDamage(dashDamage);
                }

                // ЗВУК УДАРА С ПИТЧЕМ!
                PlaySoundDirect(impactSound, impactVolume, impactPitch);
            }

            float returnTimer = 0;
            while (returnTimer < dashDuration)
            {
                returnTimer += Time.deltaTime;
                float t = returnTimer / dashDuration;
                float easedT = 1f - (1f - t) * (1f - t) * (1f - t);
                transform.position = Vector3.Lerp(targetPos, originalPos, easedT);
                yield return null;
            }

            _isDashing = false;
        }

        private void PlaySoundDirect(AudioClip clip, float volume, float pitch)
        {
            if (clip == null) return;

            var soundObj = new GameObject("DashSound");
            soundObj.transform.position = transform.position;

            var audioSource = soundObj.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch; // ИСПОЛЬЗУЕМ ПИТЧ!
            audioSource.spatialBlend = 0f;

            audioSource.Play();
            Destroy(soundObj, clip.length);
        }
    }
}