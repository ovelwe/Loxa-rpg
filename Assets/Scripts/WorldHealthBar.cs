using UnityEngine;

public class WorldHealthBar : MonoBehaviour
{
    public Transform fillTransform; // Трансформ заполнения бара
    public SpriteRenderer fillSpriteRenderer; // Спрайт рендерер заполнения
    
    private Vector3 initialFillScale;
    private Vector3 initialFillPosition;
    
    void Start()
    {
        if (fillTransform != null)
        {
            initialFillScale = fillTransform.localScale;
            initialFillPosition = fillTransform.localPosition;
        }
    }
    
    // Метод для обновления ХП бара
    public void SetHealth(float currentHealth, float maxHealth)
    {
        if (fillTransform == null || fillSpriteRenderer == null)
        {
            Debug.LogError("WorldHealthBar: Не назначены fillTransform или fillSpriteRenderer!");
            return;
        }
        
        // Вычисляем процент здоровья
        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        
        // Меняем масштаб заполнения
        Vector3 newScale = initialFillScale;
        newScale.x = initialFillScale.x * healthPercent;
        fillTransform.localScale = newScale;
        
        // Сдвигаем позицию, чтобы бар уменьшался слева
        Vector3 newPosition = initialFillPosition;
        newPosition.x = initialFillPosition.x - (initialFillScale.x * (1 - healthPercent)) / 2;
        fillTransform.localPosition = newPosition;
        
        // Меняем цвет в зависимости от здоровья
        if (healthPercent > 0.6f)
        {
            fillSpriteRenderer.color = Color.green; // Много ХП - зелёный
        }
        else if (healthPercent > 0.3f)
        {
            fillSpriteRenderer.color = Color.yellow; // Средне - жёлтый
        }
        else
        {
            fillSpriteRenderer.color = Color.red; // Мало - красный
        }
    }
}