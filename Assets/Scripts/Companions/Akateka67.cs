using UnityEngine;
using System.Collections;
using LoxaRPG.Enemies.Components;
using LoxaRPG.Systems;

namespace LoxaRPG.Companions
{
    /// <summary>
    /// Акатека67 — боевой петух-компаньон.
    /// Летает вокруг игрока, атакует ближайших зомби.
    /// </summary>
    public class Akateka67 : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private float attackDamage = 25f;
        [SerializeField] private float attackRadius = 5f;
        [SerializeField] private float orbitSpeed = 90f;
        [SerializeField] private float orbitRadius = 2f;
        [SerializeField] private float orbitHeight = 2f;
        [SerializeField] private float searchDelay = 0.5f;

        [Header("Звуки")]
        [SerializeField] private AudioClip attackSound;
        [Range(0f, 10f)] [SerializeField] private float attackSoundVolume = 2f;

        private Transform _player;
        private GameObject _targetZombie;
        private bool _isAttacking;
        private float _orbitAngle;
        private float _spawnTime;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spawnTime = Time.time;
            _spriteRenderer = GetComponent<SpriteRenderer>();

            FindPlayer();

            if (_player != null)
            {
                transform.position = _player.position + Vector3.up * orbitHeight;
            }

            _orbitAngle = Random.Range(0f, 360f);

            StartCoroutine(AkatekaBehaviour());
        }

        private void FindPlayer()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
            }
        }

        private IEnumerator AkatekaBehaviour()
        {
            // Небольшая пауза перед началом
            yield return new WaitForSeconds(0.3f);

            while (Time.time - _spawnTime < lifeTime)
            {
                if (!_isAttacking && _player != null)
                {
                    OrbitAroundPlayer();

                    if (Time.time - _spawnTime > searchDelay)
                    {
                        _targetZombie = FindNearestZombie();

                        if (_targetZombie != null)
                        {
                            _isAttacking = true;
                            yield return StartCoroutine(AttackTarget());
                            _isAttacking = false;
                        }
                    }
                }

                yield return null;
            }

            // Плавное исчезновение
            yield return StartCoroutine(FadeOut());
            Destroy(gameObject);
        }

        private void OrbitAroundPlayer()
        {
            if (_player == null) return;

            _orbitAngle += orbitSpeed * Time.deltaTime;
            if (_orbitAngle >= 360f) _orbitAngle -= 360f;

            var radians = _orbitAngle * Mathf.Deg2Rad;
            var orbitPosition = new Vector3(
                Mathf.Cos(radians) * orbitRadius,
                Mathf.Sin(radians) * orbitRadius * 0.3f + orbitHeight,
                0
            );

            transform.position = _player.position + orbitPosition;
            transform.rotation = Quaternion.identity; // всегда смотрим вверх
        }

        private GameObject FindNearestZombie()
        {
            var zombies = Physics2D.OverlapCircleAll(transform.position, attackRadius);
            GameObject nearest = null;
            float nearestDistance = Mathf.Infinity;

            foreach (var collider in zombies)
            {
                if (!collider.CompareTag("Zombie")) continue;

                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = collider.gameObject;
                }
            }

            return nearest;
        }

        private IEnumerator AttackTarget()
        {
            if (_targetZombie == null) yield break;

            var startPosition = transform.position;
            var targetPosition = _targetZombie.transform.position;

            // Полёт к зомби
            float flyTimer = 0;
            const float flyDuration = 0.3f;

            while (flyTimer < flyDuration)
            {
                flyTimer += Time.deltaTime;
                float t = flyTimer / flyDuration;
                float easedT = t * t; // Ease-In

                transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
                yield return null;
            }

            DealDamage();
        }

        private void DealDamage()
        {
            PlayAttackSound();

            if (_targetZombie != null && _targetZombie.TryGetComponent<ZombieHealth>(out var health))
            {
                health.TakeDamage(attackDamage);
            }

            // Вспышка при ударе
            CreateFlash();
        }

        private void PlayAttackSound()
        {
            if (attackSound == null) return;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(attackSound, transform.position, attackSoundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(attackSound, transform.position);
            }
        }

        private void CreateFlash()
        {
            var flash = new GameObject("Flash");
            flash.transform.position = transform.position;

            var renderer = flash.AddComponent<SpriteRenderer>();
            renderer.color = Color.yellow;
            flash.transform.localScale = Vector3.one * 2f;

            Destroy(flash, 0.2f);
        }

        private IEnumerator FadeOut()
        {
            const float duration = 0.3f;
            float timer = 0;
            var startScale = transform.localScale;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }
        }
    }
}