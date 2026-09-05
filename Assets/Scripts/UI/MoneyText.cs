using TMPro;
using UnityEngine;
using LoxaRPG.Player.Components;

namespace LoxaRPG.UI
{
    /// <summary>
    /// Отображает деньги игрока в UI.
    /// Подписывается на PlayerWallet и обновляет текст.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class MoneyText : MonoBehaviour
    {
        [SerializeField] private PlayerWallet playerWallet; // ссылка на кошелёк игрока

        private TMP_Text _text;

        private void OnEnable()
        {
            _text = GetComponent<TMP_Text>();

            // Подписываемся на изменение денег.
            if (playerWallet != null)
                playerWallet.OnMoneyChanged.AddListener(UpdateMoney);
        }

        private void OnDisable()
        {
            // Отписываемся, чтобы не было утечек.
            if (playerWallet != null)
                playerWallet.OnMoneyChanged.RemoveListener(UpdateMoney);
        }

        private void Start()
        {
            // При старте показываем текущее бабло.
            if (playerWallet != null)
                UpdateMoney(playerWallet.CurrentMoney);
        }

        private void UpdateMoney(int amount)
        {
            _text.text = amount.ToString(); // просто выводим циферки
        }
    }
}