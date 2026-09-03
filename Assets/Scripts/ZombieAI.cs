using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public float speed = 2f;
    public int damage = 10;
    public float attackCooldown = 1f;
    private Transform player;
    private float lastAttackTime;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}