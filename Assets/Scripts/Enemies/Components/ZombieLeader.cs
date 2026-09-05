using UnityEngine;

namespace LoxaRPG.Enemies.Components
{
    /// <summary>
    /// Зомби-лидер. Ускоряет зомби вокруг себя.
    /// Использует ZombieMovement вместо прямого изменения скорости.
    /// </summary>
    public class ZombieLeader : MonoBehaviour
    {
        [SerializeField] private float leadershipRadius = 5f;
        [SerializeField] private float speedBoostMultiplier = 1.5f;

        private void Update()
        {
            var nearbyZombies = Physics2D.OverlapCircleAll(transform.position, leadershipRadius);

            foreach (var collider in nearbyZombies)
            {
                if (!collider.CompareTag("Zombie") || collider.gameObject == gameObject)
                    continue;

                // Вместо прямого изменения скорости — используем компонент движения
                if (collider.TryGetComponent<ZombieMovement>(out var movement))
                {
                    movement.ApplySpeedBoost(speedBoostMultiplier);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, leadershipRadius);
        }
    }
}