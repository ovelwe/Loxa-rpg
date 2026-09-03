using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBar : MonoBehaviour
{
    public Transform fillTransform; // Трансформ заполнения бара
    public Image fillImage; // Спрайт рендерер заполнения
    
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
        if (fillTransform == null || fillImage == null)
        {
            Debug.LogError("WorldHealthBar: Не назначены fillTransform или fillSpriteRenderer!");
            return;
        }
        
        // Вычисляем процент здоровья
        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        
        fillImage.fillAmount = healthPercent;
        
        // Меняем цвет в зависимости от здоровья
        if (healthPercent > 0.6f)
        {
            fillImage.color = Color.green; // Много ХП - зелёный
        }
        else if (healthPercent > 0.3f)
        {
            fillImage.color = Color.yellow; // Средне - жёлтый
        }
        else
        {
            fillImage.color = Color.red; // Мало - красный
        }
    }
}