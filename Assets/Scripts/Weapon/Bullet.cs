using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private float speed;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(float dmg, float bulletSpeed)
    {
        damage = dmg;
        speed = bulletSpeed;
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Игнорируем игрока и другие пули
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
            return;
        
        // Проверяем обычного зомби
        ZombieHealth zombieHealth = other.GetComponent<ZombieHealth>();
        if (zombieHealth != null)
        {
            zombieHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        
        // Проверяем Гришоида (босса)
        GrishoidBoss boss = other.GetComponent<GrishoidBoss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        
        // // Если это стена или что-то ещё - уничтожаем пулю
        // if (other.CompareTag("Wall") || other.CompareTag("Ground"))
        // {
        //     Destroy(gameObject);
        // }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Игнорируем игрока и другие пули
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bullet"))
            return;
        
        // Проверяем обычного зомби
        ZombieHealth zombieHealth = collision.gameObject.GetComponent<ZombieHealth>();
        if (zombieHealth != null)
        {
            zombieHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        
        // Проверяем Гришоида (босса)
        GrishoidBoss boss = collision.gameObject.GetComponent<GrishoidBoss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
        
        // Если это стена или что-то ещё - уничтожаем пулю
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}