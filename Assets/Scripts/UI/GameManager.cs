using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int money = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // чтобы не удалялся при перезагрузке сцены
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Обновляем UI при старте
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMoneyText(money);
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        // Обновить UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMoneyText(money);
        }
        else
        {
            Debug.LogError("UIManager.Instance is null! Забыл создать UIManager на сцене?");
        }
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateMoneyText(money);
            }
            return true;
        }
        return false;
    }
}