using UnityEngine;

namespace LoxaRPG.Enemies.Bosses.Grishoid
{
    /// <summary>
    /// Движение Гришоида.
    /// Умеет ходить к игроку и останавливаться.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class GrishoidMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;

        private Transform _player;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            // Замораживаем вращение, чтобы босс не крутился как блядь на пилоне
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void SetTarget(Transform target)
        {
            _player = target;
        }

        /// <summary>
        /// Идти к игроку.
        /// </summary>
        public void MoveTowardsPlayer()
        {
            if (_player == null) return;

            var direction = (_player.position - transform.position).normalized;
            _rb.linearVelocity = direction * moveSpeed;

            FlipSprite(direction.x);
        }

        /// <summary>
        /// Остановиться.
        /// </summary>
        public void Stop()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        private void FlipSprite(float directionX)
        {
            if (_spriteRenderer == null) return;

            if (directionX > 0.1f)
                _spriteRenderer.flipX = false;
            else if (directionX < -0.1f)
                _spriteRenderer.flipX = true;
        }
    }
}