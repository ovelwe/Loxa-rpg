using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public static DamagePopup Instance { get; private set; }
    
    public GameObject damageTextPrefab; // Префаб текста урона
    public float lifeTime = 1f;
    public float moveSpeed = 2f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ShowDamage(Vector3 position, float damageAmount)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogError("DamagePopup: Не назначен префаб текста урона!");
            return;
        }
        
        // Создаём объект с текстом
        GameObject damageObj = Instantiate(damageTextPrefab, position, Quaternion.identity);
        
        // Добавляем компонент для анимации
        DamageTextAnimation anim = damageObj.GetComponent<DamageTextAnimation>();
        if (anim == null)
        {
            anim = damageObj.AddComponent<DamageTextAnimation>();
        }
        
        anim.Setup(damageAmount, lifeTime, moveSpeed);
    }
}

// Вспомогательный класс для анимации урона
