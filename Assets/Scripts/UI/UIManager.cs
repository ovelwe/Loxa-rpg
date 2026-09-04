using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text moneyText;
    public GameObject caseRewardPanel;
    public TMP_Text rewardText;
    public Image rewardImage;
    public Text warningText; // UI текст для предупреждений

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowBossWarning(string message)
    {
        // Если есть текст для предупреждений
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);
            Invoke("HideBossWarning", 2f);
        }
    }

    void HideBossWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }
    
    void Start()
    {
        // Если GameManager уже существует, обновляем текст
        if (GameManager.Instance != null)
        {
            UpdateMoneyText(GameManager.Instance.money);
        }
        else
        {
            UpdateMoneyText(0);
        }
    }

    public void UpdateMoneyText(int amount)
    {
        if (moneyText != null)
        {
            moneyText.text = "Монеты: " + amount;
        }
        else
        {
            Debug.LogError("MoneyText не назначен в UIManager!");
        }
    }

    public void ShowCaseReward(CaseItem item)
    {
        if (caseRewardPanel != null)
        {
            caseRewardPanel.SetActive(true);
        }
        
        if (rewardText != null)
        {
            rewardText.text = "Ты выбил: " + item.itemName;
        }
        
        if (rewardImage != null && item.icon != null)
        {
            rewardImage.sprite = item.icon;
        }
        
        // Через 2 секунды скрыть
        Invoke("HideReward", 2f);
    }

    void HideReward()
    {
        if (caseRewardPanel != null)
        {
            caseRewardPanel.SetActive(false);
        }
    }
}