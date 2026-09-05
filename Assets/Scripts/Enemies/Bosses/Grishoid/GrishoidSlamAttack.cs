using UnityEngine;
using System.Collections;
using LoxaRPG.Player.Components;

namespace LoxaRPG.Enemies.Bosses.Grishoid
{
    /// <summary>
    /// Атака по площади Гришоида.
    /// Звук с настраиваемым питчем.
    /// </summary>
    public class GrishoidSlamAttack : MonoBehaviour
    {
        [Header("Настройки Slam")]
        [SerializeField] private float slamRadius = 3f;
        [SerializeField] private int slamDamage = 15;
        [SerializeField] private float slamCooldown = 5f;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] private float jumpDuration = 0.3f;
        [SerializeField] private float fallDuration = 0.3f;

        [Header("Стан")]
        [SerializeField] private float stunDuration = 1f;

        [Header("Звук")]
        [SerializeField] private AudioClip slamSound;
        [Range(0f, 5f)] [SerializeField] private float soundVolume = 2f;
        [Range(0.1f, 3f)] [SerializeField] private float soundPitch = 1f; // ПИТЧ СЛЭМА

        [Header("Волна")]
        [SerializeField] private Color waveColor = new Color(0.5f, 0.7f, 1f, 0.8f);
        [SerializeField] private float waveDuration = 0.5f;

        private Transform _player;
        private float _lastSlamTime;
        private bool _isSlamming;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalColor = _spriteRenderer.color;

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

        public bool CanSlam()
        {
            return !_isSlamming && Time.time >= _lastSlamTime + slamCooldown;
        }

        public void StartSlam()
        {
            if (!CanSlam()) return;
            StartCoroutine(SlamRoutine());
        }

        private IEnumerator SlamRoutine()
        {
            _isSlamming = true;
            _lastSlamTime = Time.time;

            for (int i = 0; i < 3; i++)
            {
                _spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                _spriteRenderer.color = _originalColor;
                yield return new WaitForSeconds(0.1f);
            }

            var startPos = transform.position;
            var jumpPos = startPos + Vector3.up * jumpHeight;

            float jumpTimer = 0;
            while (jumpTimer < jumpDuration)
            {
                jumpTimer += Time.deltaTime;
                float t = jumpTimer / jumpDuration;
                float easedT = t * t;
                transform.position = Vector3.Lerp(startPos, jumpPos, easedT);
                yield return null;
            }

            float fallTimer = 0;
            while (fallTimer < fallDuration)
            {
                fallTimer += Time.deltaTime;
                float t = fallTimer / fallDuration;
                float easedT = 1f - (1f - t) * (1f - t);
                transform.position = Vector3.Lerp(jumpPos, startPos, easedT);
                yield return null;
            }

            // ЗВУК С ПИТЧЕМ ПОСЛЕ ПАДЕНИЯ!
            PlaySoundDirect(slamSound);

            DealDamageAndStun();
            CreateShockwave();

            _isSlamming = false;
        }

        private void DealDamageAndStun()
        {
            if (_player == null) return;

            float distance = Vector3.Distance(transform.position, _player.position);

            if (distance <= slamRadius)
            {
                if (_player.TryGetComponent<PlayerHealth>(out var health))
                {
                    health.TakeDamage(slamDamage);
                }

                if (_player.TryGetComponent<StunEffect>(out var stun))
                {
                    stun.StunPlayer(stunDuration);
                }
            }
        }

        private void CreateShockwave()
        {
            var waveObj = new GameObject("SlamShockwave");
            waveObj.transform.position = transform.position;

            var waveRenderer = waveObj.AddComponent<SpriteRenderer>();
            waveRenderer.sprite = CreateCircleSprite();
            waveRenderer.color = waveColor;
            waveRenderer.sortingOrder = 100;

            waveObj.transform.localScale = Vector3.one * 0.5f;

            StartCoroutine(AnimateShockwave(waveObj, waveRenderer));
        }

        private IEnumerator AnimateShockwave(GameObject waveObj, SpriteRenderer waveRenderer)
        {
            float timer = 0;
            var startScale = Vector3.one * 0.5f;
            var endScale = Vector3.one * (slamRadius * 2f);

            while (timer < waveDuration)
            {
                timer += Time.deltaTime;
                float t = timer / waveDuration;

                waveObj.transform.localScale = Vector3.Lerp(startScale, endScale, t);

                var color = waveRenderer.color;
                color.a = Mathf.Lerp(0.8f, 0f, t);
                waveRenderer.color = color;

                yield return null;
            }

            Destroy(waveObj);
        }

        private Sprite CreateCircleSprite()
        {
            int size = 128;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

            var center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);

                    if (distance < radius)
                    {
                        float alpha = Mathf.Clamp01((radius - distance) / 3f);
                        texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        private void PlaySoundDirect(AudioClip clip)
        {
            if (clip == null) return;

            var soundObj = new GameObject("SlamSound");
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