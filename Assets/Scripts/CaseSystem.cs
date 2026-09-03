using UnityEngine;

[System.Serializable]
public class CaseItem
{
    public string itemName;
    public Sprite icon;
    [Range(0f, 100f)] public float dropChance;
}

public class CaseSystem : MonoBehaviour
{
    public CaseItem[] items;
    public int caseCost = 50; // в рублях (имитация)
    public GameObject caseOpenPanel; // UI панель

    public void BuyCase()
    {
        // Здесь в реальной игре вызвать платёжный SDK
        // Для теста просто спишем виртуальные деньги (или добавим, если покупка за реал)
        Debug.Log("Покупка кейса за " + caseCost + " руб. (имитация)");
        // Допустим, игрок получает кейс и сразу открывает
        OpenCase();
    }

    public void OpenCase()
    {
        float totalWeight = 0;
        foreach (var item in items) totalWeight += item.dropChance;

        float random = Random.Range(0, totalWeight);
        float cumulative = 0;
        CaseItem droppedItem = items[0];

        foreach (var item in items)
        {
            cumulative += item.dropChance;
            if (random <= cumulative)
            {
                droppedItem = item;
                break;
            }
        }

        // Показываем награду
        UIManager.Instance.ShowCaseReward(droppedItem);
        // Применяем предмет (например, скин или буст)
        ApplyItem(droppedItem);
    }

    void ApplyItem(CaseItem item)
    {
        Debug.Log("Выпал предмет: " + item.itemName);
        // Здесь можно менять характеристики оружия, цвет петуха и т.д.
    }
}