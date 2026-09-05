using UnityEngine;

namespace LoxaRPG.Enemies.Components
{
    /// <summary>
    /// Движение зомби.
    /// Умеет ходить к цели, останавливаться и получать ускорение от лидера.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class ZombieMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f; // базовая скорость

        private Rigidbody2D _rb;
        private Transform _target;
        private float _speedMultiplier = 1f; // множитель скорости от лидера

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation; // чтобы не крутился
        }

        /// <summary>
        /// Задать цель, к которой будем идти.
        /// </summary>
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// Идти к цели.
        /// </summary>
        public void MoveTowardsTarget()
        {
            if (_target == null) return;

            var direction = (_target.position - transform.position).normalized;
            _rb.linearVelocity = direction * moveSpeed * _speedMultiplier;
        }

        /// <summary>
        /// Остановиться нахуй.
        /// </summary>
        public void Stop()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        /// <summary>
        /// Применить ускорение от зомби-лидера.
        /// </summary>
        public void ApplySpeedBoost(float multiplier)
        {
            _speedMultiplier = multiplier;
        }

        /// <summary>
        /// Сбросить ускорение.
        /// </summary>
        public void ResetSpeedBoost()
        {
            _speedMultiplier = 1f;
        }
    }
}