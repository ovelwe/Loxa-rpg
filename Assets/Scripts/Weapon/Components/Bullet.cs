using UnityEngine;
using LoxaRPG.Enemies.Components;
using LoxaRPG.Enemies.Bosses.Grishoid;

namespace LoxaRPG.Weapons.Components
{
    /// <summary>
    /// Пуля. Летит и наносит урон всему, что можно ударить.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        private float _damage;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Init(float damage, float speed)
        {
            _damage = damage;
            _rb.linearVelocity = transform.right * speed;
            Destroy(gameObject, 3f); // живёт 3 секунды
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Игнорируем игрока и другие пули
            if (other.CompareTag("Player") || other.CompareTag("Bullet"))
                return;

            // Бьём зомби
            if (other.TryGetComponent<ZombieHealth>(out var zombieHealth))
            {
                zombieHealth.TakeDamage(_damage);
                Destroy(gameObject);
                return;
            }

            // Бьём босса
            if (other.TryGetComponent<GrishoidHealth>(out var bossHealth))
            {
                bossHealth.TakeDamage(_damage);
                Destroy(gameObject);
                return;
            }

            // Стены и прочее
            if (other.CompareTag("Wall") || other.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}