using UnityEngine;
using System.Collections;

public class Akateka67 : MonoBehaviour
{
    [Header("Настройки Акатеки")]
    public float lifeTime = 3f;
    public float attackDamage = 25f;
    public float attackRadius = 5f;
    public float orbitSpeed = 90f;
    public float orbitRadius = 2f;
    public float orbitHeight = 2f; // Высота над игроком (увеличим)
    public float searchDelay = 0.5f; // Задержка перед поиском зомби
    
    [Header("Звуки")]
    public AudioClip attackSound;
    [Range(0f, 10f)] public float attackSoundVolume = 2f;
    
    private Transform player;
    private GameObject targetZombie;
    private bool isAttacking = false;
    private float orbitAngle = 0f;
    private float spawnTime;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spawnTime = Time.time;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Находим игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        // СТАВИМ АКАТЕКУ СТРОГО НАД ИГРОКОМ
        if (player != null)
        {
            transform.position = player.position + Vector3.up * orbitHeight;
            Debug.Log("Акатека67: Спавн над игроком на высоте " + orbitHeight);
        }
        
        // Начальный угол
        orbitAngle = Random.Range(0f, 360f);
        
        // Запускаем поведение
        StartCoroutine(AkatekaBehavior());
    }
    
    IEnumerator AkatekaBehavior()
    {
        Debug.Log("Акатека67: Появился!");
        
        // Ждём перед началом (чтобы не сразу в игрока)
        yield return new WaitForSeconds(0.3f);
        
        // Основной цикл
        while (Time.time - spawnTime < lifeTime)
        {
            if (!isAttacking && player != null)
            {
                // Летаем вокруг игрока
                OrbitAroundPlayer();
                
                // Ищем зомби только после задержки
                if (Time.time - spawnTime > searchDelay)
                {
                    targetZombie = FindNearestZombie();
                    
                    if (targetZombie != null)
                    {
                        Debug.Log("Акатека67: Нашёл зомби, атакую!");
                        isAttacking = true;
                        yield return StartCoroutine(AttackTarget());
                        isAttacking = false;
                    }
                }
            }
            
            yield return null;
        }
        
        Debug.Log("Акатека67: Время вышло, исчезаю");
        
        // Плавное исчезновение
        float disappearDuration = 0.3f;
        float disappearTimer = 0;
        Vector3 startScale = transform.localScale;
        
        while (disappearTimer < disappearDuration)
        {
            disappearTimer += Time.deltaTime;
            float t = disappearTimer / disappearDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    void OrbitAroundPlayer()
    {
        if (player == null) return;
        
        // Увеличиваем угол
        orbitAngle += orbitSpeed * Time.deltaTime;
        if (orbitAngle >= 360f) orbitAngle -= 360f;
        
        // Позиция на орбите (ВОКРУГ и НАД игроком)
        float radians = orbitAngle * Mathf.Deg2Rad;
        Vector3 orbitPosition = new Vector3(
            Mathf.Cos(radians) * orbitRadius,
            Mathf.Sin(radians) * orbitRadius * 0.3f + orbitHeight, // Летает ВЫШЕ игрока
            0
        );
        
        // Перемещаем Акатеку
        transform.position = player.position + orbitPosition;
        
        // НЕ ПОВОРАЧИВАЕМ — пусть всегда смотрит вверх
        transform.rotation = Quaternion.identity;
    }
    
    GameObject FindNearestZombie()
    {
        // Ищем зомби вокруг Акатеки
        Collider2D[] zombies = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        
        GameObject nearestZombie = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (Collider2D collider in zombies)
        {
            if (collider.CompareTag("Zombie"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestZombie = collider.gameObject;
                }
            }
        }
        
        if (nearestZombie != null)
        {
            Debug.Log("Акатека67: Зомби найден на дистанции " + nearestDistance);
        }
        
        return nearestZombie;
    }
    
    IEnumerator AttackTarget()
    {
        if (targetZombie == null) yield break;
        
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = targetZombie.transform.position;
        
        Debug.Log("Акатека67: Лечу к зомби!");
        
        // Полёт к зомби
        float flyDuration = 0.3f;
        float flyTimer = 0;
        
        while (flyTimer < flyDuration)
        {
            flyTimer += Time.deltaTime;
            float t = flyTimer / flyDuration;
            float easedT = t * t;
            
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            yield return null;
        }
        
        Debug.Log("Акатека67: Бью зомби!");
        
        // Наносим урон
        DealDamage();
    }
    
    void DealDamage()
    {
        // Звук
        if (attackSound != null)
        {
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
        
        // Урон зомби
        if (targetZombie != null)
        {
            ZombieHealth zombieHealth = targetZombie.GetComponent<ZombieHealth>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(attackDamage);
                Debug.Log("Акатека67: Нанёс " + attackDamage + " урона!");
            }
        }
        
        // Вспышка
        GameObject flash = new GameObject("Flash");
        flash.transform.position = transform.position;
        SpriteRenderer flashRenderer = flash.AddComponent<SpriteRenderer>();
        flashRenderer.color = Color.yellow;
        flash.transform.localScale = Vector3.one * 2f;
        Destroy(flash, 0.2f);
    }
}