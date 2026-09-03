using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private float speed;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //
    }

    public void Init(float dmg, float bulletSpeed)
    {
        damage = dmg;
        speed = bulletSpeed;
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, 2f); // пуля живёт 2 сек
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Zombie"))
        {
            other.GetComponent<ZombieHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}