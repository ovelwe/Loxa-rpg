using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GrishoidBoss : MonoBehaviour
{
    [Header("Основные характеристики")]
    public float maxHealth = 300f;
    public float moveSpeed = 3f;
    public float attackDamage = 20f;
    public float attackRange = 5f;
    public float detectionRange = 15f;
    
    [Header("Атака вылетом")]
    public float dashSpeed = 15f;
    public float dashDistance = 6f;
    public float dashCooldown = 3f;
    public float dashDuration = 0.3f;
    
    [Header("Способности")]
    public float slamRadius = 3f;
    public float slamDamage = 15f;
    public float slamCooldown = 5f;
    public float slamJumpHeight = 3f; // Высота прыжка
    public float slamJumpDuration = 0.3f; // Длительность прыжка
    public float slamFallDuration = 0.3f; // Длительность падения
    
    [Header("ХП Бар")]
    public Slider healthBarSlider;
    public GameObject healthBarObject;
    public TMP_Text bossNameText;
    
    [Header("Звуки")]
    public AudioClip dashSound;
    public AudioClip attackSound;
    public AudioClip slamSound;
    public AudioClip hurtSound;
    [Range(0f, 5f)] public float soundVolume = 2f;
    [Range(0f, 5f)] public float hurtSoundVolume = 2f;
    
    [Header("Настройка звука урона")]
    public float hurtSoundCooldown = 0.3f;
    
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float currentHealth;
    private bool isAttacking = false;
    private float lastDashTime;
    private float lastSlamTime;
    private float lastHurtSoundTime;
    private Vector3 originalPosition;
    private Color originalColor;
    private bool isDead = false; // Добавь в начало класса
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Замораживаем вращение
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;
        
        SetupHealthBar();
        FindPlayer();
        InvokeRepeating("FindPlayer", 0f, 2f);
        
        StartCoroutine(BossBehavior());
    }
    
    void SetupHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHealth;
            healthBarSlider.value = currentHealth;
        }
        
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(true);
        }
        
        if (bossNameText != null)
        {
            bossNameText.text = "ГРИШОИД";
        }
    }
    
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    IEnumerator BossBehavior()
    {
        while (true)
        {
            if (player == null || isAttacking)
            {
                yield return null;
                continue;
            }
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            if (distanceToPlayer > attackRange)
            {
                MoveTowardsPlayer();
            }
            else
            {
                ChooseAttack();
            }
            
            yield return null;
        }
    }
    
    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        
        FlipSprite(direction.x);
    }
    
    void FlipSprite(float directionX)
    {
        if (spriteRenderer == null) return;
        
        if (directionX > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (directionX < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }
    
    void ChooseAttack()
    {
        float random = Random.value;
        
        // 50% шанс Dash, 30% шанс Slam, 20% шанс Normal
        if (random < 0.5f && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(DashAttack());
        }
        else if (random < 0.8f && Time.time >= lastSlamTime + slamCooldown)
        {
            StartCoroutine(SlamAttack());
        }
        else if (Time.time >= lastDashTime + dashCooldown && Time.time >= lastSlamTime + slamCooldown)
        {
            StartCoroutine(NormalAttack());
        }
        else
        {
            // Если все на кулдауне - просто двигаемся
            MoveTowardsPlayer();
        }
    }
    
    IEnumerator DashAttack()
    {
        isAttacking = true;
        lastDashTime = Time.time;
        
        rb.linearVelocity = Vector2.zero;
        originalPosition = transform.position;
        
        Vector2 dashDirection = (player.position - transform.position).normalized;
        Vector3 targetPosition = player.position;
        
        PlaySound(dashSound);
        FlipSprite(dashDirection.x);
        
        // Полёт к игроку
        float flyTimer = 0;
        while (flyTimer < dashDuration)
        {
            flyTimer += Time.deltaTime;
            float t = flyTimer / dashDuration;
            float easedT = t * t * (3f - 2f * t);
            
            transform.position = Vector3.Lerp(originalPosition, targetPosition, easedT);
            
            // Мигаем белым при вылете
            spriteRenderer.color = flyTimer < 0.1f ? Color.white : originalColor;
            
            yield return null;
        }
        
        // Проверяем попадание
        float distanceAfterDash = Vector2.Distance(transform.position, player.position);
        if (distanceAfterDash < 2f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)attackDamage);
                Debug.Log("Гришоид вылетел в игрока и нанёс " + attackDamage + " урона!");
            }
        }
        
        PlaySound(attackSound);
        
        // Возврат на место
        float returnTimer = 0;
        while (returnTimer < dashDuration)
        {
            returnTimer += Time.deltaTime;
            float t = returnTimer / dashDuration;
            float easedT = 1f - (1f - t) * (1f - t) * (1f - t);
            
            transform.position = Vector3.Lerp(targetPosition, originalPosition, easedT);
            
            yield return null;
        }
        
        spriteRenderer.color = originalColor;
        isAttacking = false;
        
        yield return new WaitForSeconds(0.5f);
    }
    
    IEnumerator SlamAttack()
    {
        isAttacking = true;
        lastSlamTime = Time.time;
        
        rb.linearVelocity = Vector2.zero;
        
        // Мигаем красным (предупреждение)
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
        
        // Прыжок вверх
        Vector3 originalPos = transform.position;
        Vector3 jumpPos = originalPos + Vector3.up * slamJumpHeight;
        
        float jumpTimer = 0;
        while (jumpTimer < slamJumpDuration)
        {
            jumpTimer += Time.deltaTime;
            float t = jumpTimer / slamJumpDuration;
            float easedT = t * t; // Ease-In
            transform.position = Vector3.Lerp(originalPos, jumpPos, easedT);
            yield return null;
        }
        
        // Падение вниз
        float fallTimer = 0;
        while (fallTimer < slamFallDuration)
        {
            fallTimer += Time.deltaTime;
            float t = fallTimer / slamFallDuration;
            float easedT = 1f - (1f - t) * (1f - t); // Ease-Out
            transform.position = Vector3.Lerp(jumpPos, originalPos, easedT);
            yield return null;
        }
        
        // ЗВУК ПОСЛЕ ПАДЕНИЯ!
        PlaySound(slamSound);
        
        // Наносим урон
        DealSlamDamage();
        
        // Создаём волну
        CreateShockwave();
        
        isAttacking = false;
        
        yield return new WaitForSeconds(0.5f);
    }
    
    void DealSlamDamage()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= slamRadius)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)slamDamage);
                Debug.Log("Гришоид ударил по площади и нанёс " + slamDamage + " урона!");
            }
            
            StunEffect stunEffect = player.GetComponent<StunEffect>();
            if (stunEffect != null)
            {
                stunEffect.StunPlayer(1f);
                Debug.Log("Игрок оглушён!");
            }
        }
    }
    
    IEnumerator NormalAttack()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        
        // Замах
        Vector3 originalPos = transform.position;
        Vector3 windupPos = originalPos + Vector3.up * 0.5f;
        
        float windupTimer = 0;
        while (windupTimer < 0.2f)
        {
            windupTimer += Time.deltaTime;
            float t = windupTimer / 0.2f;
            transform.position = Vector3.Lerp(originalPos, windupPos, t);
            yield return null;
        }
        
        // Удар
        PlaySound(attackSound);
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < 2.5f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)(attackDamage * 0.7f));
            }
        }
        
        // Возврат
        float returnTimer = 0;
        while (returnTimer < 0.2f)
        {
            returnTimer += Time.deltaTime;
            float t = returnTimer / 0.2f;
            transform.position = Vector3.Lerp(windupPos, originalPos, t);
            yield return null;
        }
        
        isAttacking = false;
        yield return new WaitForSeconds(0.5f);
    }
    
    void CreateShockwave()
    {
        GameObject shockwave = new GameObject("StunShockwave");
        shockwave.transform.position = transform.position;
    
        SpriteRenderer waveRenderer = shockwave.AddComponent<SpriteRenderer>();
        waveRenderer.sprite = CreateCircleSprite();
        waveRenderer.color = new Color(0.5f, 0.7f, 1f, 0.8f);
        waveRenderer.sortingOrder = 100;
    
        shockwave.transform.localScale = Vector3.one * 0.5f;
    
        // Добавляем скрипт для автономной анимации
        ShockwaveAnimation shockwaveAnim = shockwave.AddComponent<ShockwaveAnimation>();
        shockwaveAnim.Initialize(slamRadius * 2f, 0.5f);
    
        // Запускаем анимацию в отдельном объекте
        shockwaveAnim.StartAnimation();
    }
    
    IEnumerator AnimateShockwave(GameObject shockwave)
    {
        float duration = 0.5f;
        float timer = 0;
    
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale = Vector3.one * (slamRadius * 2f);
    
        while (timer < duration)
        {
            // Если босс умер - уничтожаем волну
            if (isDead || shockwave == null)
            {
                Destroy(shockwave);
                yield break;
            }
        
            timer += Time.deltaTime;
            float t = timer / duration;
        
            shockwave.transform.localScale = Vector3.Lerp(startScale, endScale, t);
        
            SpriteRenderer renderer = shockwave.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = 0.8f * (1f - t);
                renderer.color = color;
            }
        
            yield return null;
        }
    
        if (shockwave != null)
        {
            Destroy(shockwave);
        }
    }
    
    Sprite CreateCircleSprite()
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance < radius)
                {
                    // Мягкий край
                    float alpha = Mathf.Clamp01((radius - distance) / 2f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
    
    void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySoundNoCooldown(clip, transform.position, soundVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
    
    public void TakeDamage(float amount)
    {
        if (isDead) return; // Если уже мёртв - игнорируем урон
    
        currentHealth -= amount;
        Debug.Log("Гришоид получил урон: " + amount + ", осталось HP: " + currentHealth);
    
        PlayHurtSound();
        UpdateHealthBar();
        StartCoroutine(FlashWhite());
    
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void PlayHurtSound()
    {
        // Кулдаун для звука урона
        if (Time.time < lastHurtSoundTime + hurtSoundCooldown)
        {
            return;
        }
        
        lastHurtSoundTime = Time.time;
        
        if (hurtSound == null) return;
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHurtSound(hurtSound, transform.position, hurtSoundVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        }
    }
    
    void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }
    }
    
    IEnumerator FlashWhite()
    {
        if (spriteRenderer == null) yield break;
        
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    
    void Die()
    {
        if (isDead) return; // Защита от двойного вызова
        isDead = true;
    
        Debug.Log("ГРИШОИД ПОВЕРЖЕН!");
    
        // Останавливаем ВСЕ корутины
        StopAllCoroutines();
    
        // Скрываем ХП бар
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(false);
        }
    
        // Сообщаем спавнеру о смерти
        ZombieDeathReporter reporter = GetComponent<ZombieDeathReporter>();
        if (reporter != null)
        {
            reporter.ReportDeath();
        }
    
        // Награда
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(500);
        }
    
        // Уничтожаем объект сразу
        Destroy(gameObject);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, slamRadius);
    }
}