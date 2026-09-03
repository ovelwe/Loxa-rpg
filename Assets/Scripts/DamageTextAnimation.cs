using TMPro;
using UnityEngine;
public class DamageTextAnimation : MonoBehaviour
{
    private float timer = 0;
    private Vector3 moveDirection;
    private float lifeTime;
    private float moveSpeed;
    private TMP_Text textMesh;
    
    public void Setup(float damageAmount, float lifetime, float speed)
    {
        lifeTime = lifetime;
        moveSpeed = speed;
        
        // Получаем или создаём TextMesh
        textMesh = GetComponent<TMP_Text>();
        if (textMesh == null)
        {
            textMesh = gameObject.AddComponent<TMP_Text>();
        }
        
        // Настраиваем текст
        textMesh.text = Mathf.RoundToInt(damageAmount).ToString();
        textMesh.fontSize = 36;
        textMesh.color = GetColorForDamage(damageAmount);
        
        // Случайное направление разлёта
        float angle = Random.Range(-45f, 45f) + 90f;
        float radians = angle * Mathf.Deg2Rad;
        moveDirection = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0).normalized;
        
        // Поворачиваем текст к камере
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
    
    Color GetColorForDamage(float damage)
    {
        if (damage >= 20)
        {
            return Color.red; // Большой урон - красный
        }
        else if (damage >= 10)
        {
            return Color.yellow; // Средний урон - жёлтый
        }
        else
        {
            return Color.white; // Маленький урон - белый
        }
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // Двигаем вверх
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        
        // Постепенно затухаем
        if (textMesh != null && timer >= lifeTime * 0.5f)
        {
            Color color = textMesh.color;
            color.a -= Time.deltaTime * 2f;
            textMesh.color = color;
        }
        
        // Уничтожаем
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}