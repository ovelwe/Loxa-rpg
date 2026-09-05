using UnityEngine;
using UnityEngine.Events;

namespace LoxaRPG.Player.Components
{
    /// <summary>
    /// Кошелёк игрока.
    /// Только деньги и событие изменения. Никакого UI и лишней хуйни.
    /// </summary>
    public class PlayerWallet : MonoBehaviour
    {
        [SerializeField] private int startMoney = 67; // сколько бабла на старте

        public UnityEvent<int> OnMoneyChanged; // событие: деньги изменились

        public int CurrentMoney { get; private set; } // наружу только читать

        private void Awake()
        {
            CurrentMoney = startMoney; // при старте даём бабки
        }

        /// <summary>
        /// Попытаться потратить деньги.
        /// Вернёт false, если бабла не хватает.
        /// </summary>
        public bool TrySpend(int amount)
        {
            if (CurrentMoney < amount)
                return false; // денег нет, иди работай

            CurrentMoney -= amount;
            OnMoneyChanged?.Invoke(CurrentMoney);
            return true;
        }

        /// <summary>
        /// Добавить денег. Просто так, без лишних вопросов.
        /// </summary>
        public void Add(int amount)
        {
            CurrentMoney += amount;
            OnMoneyChanged?.Invoke(CurrentMoney);
        }
    }
}