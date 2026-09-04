using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    [Header("Настройки зомби")]
    public float moveSpeed = 2f;
    public float attackDamage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float detectionRange = 10f;
    
    [Header("Звуки")]
    public AudioClip attackSound; // Звук удара
    
    [Header("Громкость (можно больше 1!)")]
    [Range(0f, 5f)] public float attackSoundVolume = 2f; // Громкость удара
    
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Добавили SpriteRenderer
    private float lastAttackTime;
    private bool playerDetected = false;
    private Vector2 moveDirection;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Получаем SpriteRenderer
        FindPlayer();
        InvokeRepeating("FindPlayer", 0f, 1f);
    }
    
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Проверяем, видит ли зомби игрока
        if (distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
        }
        
        if (!playerDetected)
        {
            // Стоим на месте
            rb.linearVelocity = Vector2.zero;
            moveDirection = Vector2.zero;
            return;
        }
        
        // Если игрок далеко - идём к нему
        if (distanceToPlayer > attackRange)
        {
            MoveTowardsPlayer();
        }
        // Если близко - атакуем
        else
        {
            AttackPlayer();
        }
    }
    
    void MoveTowardsPlayer()
    {
        // Направление к игроку (только для движения, не для поворота)
        moveDirection = (player.position - transform.position).normalized;
        
        // Двигаемся
        rb.linearVelocity = moveDirection * moveSpeed;
        
        // Зеркалим спрайт через SpriteRenderer
        FlipSprite(moveDirection.x);
    }
    
    void AttackPlayer()
    {
        // Останавливаемся
        rb.linearVelocity = Vector2.zero;
        moveDirection = Vector2.zero;
        
        // Смотрим на игрока
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        FlipSprite(directionToPlayer.x);
        
        // Атакуем с кулдауном
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            
            // Проигрываем звук удара
            PlayAttackSound();
            
            // Наносим урон игроку
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)attackDamage);
                Debug.Log("Зомби ударил игрока на " + attackDamage);
            }
        }
    }
    
    // Зеркалим спрайт через SpriteRenderer.flipX
    void FlipSprite(float directionX)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer не найден на зомби!");
            return;
        }
        
        if (directionX > 0.1f)
        {
            // Игрок справа - включаем flipX
            spriteRenderer.flipX = false;
        }
        else if (directionX < -0.1f)
        {
            // Игрок слева - выключаем flipX
            spriteRenderer.flipX = true;
        }
        // Если directionX около нуля - не меняем
    }
    
    void PlayAttackSound()
    {
        if (attackSound != null)
        {
            // Создаём временный объект для ГРОМКОГО звука
            GameObject soundObject = new GameObject("LoudAttackSound");
            soundObject.transform.position = transform.position;
            
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = attackSound;
            
            // НАСТРОЙКА ГРОМКОСТИ
            audioSource.volume = attackSoundVolume; // Может быть 2, 3, 5!
            audioSource.spatialBlend = 0f; // 0 = 2D звук (слышно везде одинаково)
            audioSource.bypassEffects = true;
            audioSource.bypassListenerEffects = true;
            audioSource.bypassReverbZones = true;
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            
            audioSource.Play();
            
            // Уничтожаем после проигрывания
            Destroy(soundObject, attackSound.length);
        }
    }
    
    // Для визуализации в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}