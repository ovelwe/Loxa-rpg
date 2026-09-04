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
    public int caseCost = 67; // стоимость в монетах
    public GameObject caseOpenPanel; // UI панель
    
    [Header("Акатека67")]
    public GameObject akateka67Prefab; // Префаб Акатеки67
    
    public void BuyCase()
    {
        // Проверяем, хватает ли монет
        if (GameManager.Instance != null && GameManager.Instance.money >= caseCost)
        {
            // Списываем монеты
            GameManager.Instance.SpendMoney(caseCost);
            Debug.Log("Куплен кейс за " + caseCost + " монет");
            // Открываем кейс
            OpenCase();
        }
        else
        {
            Debug.Log("Недостаточно монет! Нужно " + caseCost + ", у тебя " + 
                     (GameManager.Instance != null ? GameManager.Instance.money : 0));
        }
    }

    public void OpenCase()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogError("Нет предметов в кейсе! Добавь их в инспекторе.");
            return;
        }

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
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCaseReward(droppedItem);
        }
        else
        {
            Debug.LogError("UIManager.Instance is null!");
        }
        
        // Применяем предмет
        ApplyItem(droppedItem);
    }

    void ApplyItem(CaseItem item)
    {
        Debug.Log("Выпал предмет: " + item.itemName);
        
        // Здесь можно применять эффекты
        switch (item.itemName)
        {
            case "Монеты +100":
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddMoney(100);
                }
                break;
                
            case "Урон +10%":
                // Найди оружие и увеличь урон
                Weapon[] weapons = FindObjectsOfType<Weapon>();
                foreach (Weapon weapon in weapons)
                {
                    weapon.damage *= 1.1f;
                }
                break;
                
            case "Акатека67":
                SpawnAkateka67();
                break;
                
            case "Скин золотой":
                // Поменяй цвет игрока
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = Color.yellow;
                    }
                }
                break;
        }
    }
    
    // Метод для спавна Акатеки67
    void SpawnAkateka67()
    {
        if (akateka67Prefab == null)
        {
            Debug.LogError("Акатека67: Префаб не назначен в CaseSystem!");
            return;
        }
    
        // Находим игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Акатека67: Игрок не найден!");
            return;
        }
    
        // Спавним Акатеку ВЫСОКО НАД игроком (3 единицы вверх)
        Vector3 spawnPosition = player.transform.position + Vector3.up * 3f;
        GameObject akateka = Instantiate(akateka67Prefab, spawnPosition, Quaternion.identity);
    
        Debug.Log("Акатека67: Заспавнен на позиции " + spawnPosition);
    }
}