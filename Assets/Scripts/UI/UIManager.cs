using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoxaRPG.UI
{
    /// <summary>
    /// Управляет основным UI: предупреждения, награды.
    /// Деньги убраны в MoneyText + PlayerWallet.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Boss Warning")]
        [SerializeField] private TMP_Text warningText;

        [Header("Reward Panel")]
        [SerializeField] private GameObject caseRewardPanel;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private Image rewardImage;

        private void Awake()
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
            if (warningText == null) return;

            warningText.text = message;
            warningText.gameObject.SetActive(true);

            CancelInvoke(nameof(HideBossWarning));
            Invoke(nameof(HideBossWarning), 2f);
        }

        private void HideBossWarning()
        {
            if (warningText != null)
                warningText.gameObject.SetActive(false);
        }

        public void ShowCaseReward(Sprite icon, string itemName)
        {
            if (caseRewardPanel != null)
                caseRewardPanel.SetActive(true);

            if (rewardText != null)
                rewardText.text = $"Ты выбил: {itemName}";

            if (rewardImage != null && icon != null)
                rewardImage.sprite = icon;

            CancelInvoke(nameof(HideReward));
            Invoke(nameof(HideReward), 2f);
        }

        private void HideReward()
        {
            if (caseRewardPanel != null)
                caseRewardPanel.SetActive(false);
        }
    }
}