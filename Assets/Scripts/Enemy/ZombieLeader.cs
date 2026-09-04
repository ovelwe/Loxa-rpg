using UnityEngine;

public class ZombieLeader : MonoBehaviour
{
    public float leadershipRadius = 5f;
    public float speedBoost = 1.5f;
    
    void Update()
    {
        // Находим всех зомби рядом
        Collider2D[] nearbyZombies = Physics2D.OverlapCircleAll(transform.position, leadershipRadius);
        
        foreach (Collider2D collider in nearbyZombies)
        {
            if (collider.CompareTag("Zombie") && collider.gameObject != gameObject)
            {
                ZombieAI zombie = collider.GetComponent<ZombieAI>();
                if (zombie != null)
                {
                    // Ускоряем зомби рядом
                    zombie.moveSpeed *= speedBoost;
                }
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, leadershipRadius);
    }
}