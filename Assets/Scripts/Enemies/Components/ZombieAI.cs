using UnityEngine;
using LoxaRPG.Player.Components;

namespace LoxaRPG.Enemies.Components
{
    /// <summary>
    /// Мозг зомби.
    /// Идёт к игроку, атакует, если рядом. Никакого здоровья и визуала.
    /// </summary>
    [RequireComponent(typeof(ZombieMovement))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ZombieAI : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private float attackDamage = 10;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float detectionRange = 10f;

        [Header("Звуки")]
        [SerializeField] private AudioClip attackSound;
        [Range(0f, 5f)] [SerializeField] private float attackSoundVolume = 2f;

        private Transform _player;
        private ZombieMovement _movement;
        private SpriteRenderer _spriteRenderer;
        private float _lastAttackTime;
        private bool _playerDetected;

        private void Awake()
        {
            _movement = GetComponent<ZombieMovement>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            FindPlayer();
            InvokeRepeating(nameof(FindPlayer), 0f, 1f); // ищем игрока каждую секунду
        }

        private void FindPlayer()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
                _movement.SetTarget(_player);
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                FindPlayer();
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

            // Если игрок в радиусе обнаружения — начинаем охоту
            if (distanceToPlayer <= detectionRange)
            {
                _playerDetected = true;
            }

            if (!_playerDetected)
            {
                _movement.Stop();
                return;
            }

            // Если далеко — идём. Если близко — атакуем.
            if (distanceToPlayer > attackRange)
            {
                _movement.MoveTowardsTarget();
            }
            else
            {
                AttackPlayer();
            }
        }

        private void AttackPlayer()
        {
            _movement.Stop();

            var directionToPlayer = (_player.position - transform.position).normalized;
            FlipSprite(directionToPlayer.x);

            // Атакуем с кулдауном
            if (Time.time < _lastAttackTime + attackCooldown) return;

            _lastAttackTime = Time.time;
            PlayAttackSound();

            if (_player.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage((int)attackDamage);
            }
        }

        /// <summary>
        /// Зеркалим спрайт через flipX. Просто и эффективно.
        /// </summary>
        private void FlipSprite(float directionX)
        {
            if (_spriteRenderer == null) return;

            if (directionX > 0.1f)
                _spriteRenderer.flipX = false; // смотрим вправо
            else if (directionX < -0.1f)
                _spriteRenderer.flipX = true; // смотрим влево
        }

        private void PlayAttackSound()
        {
            if (attackSound == null) return;

            if (LoxaRPG.Systems.SoundManager.Instance != null)
            {
                LoxaRPG.Systems.SoundManager.Instance.PlaySound(attackSound, transform.position, attackSoundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(attackSound, transform.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}