using Companions;
using UnityEngine;

namespace Player.Wallet
{
    public class PlayerWallet : MonoBehaviour
    {
        private int _currentMoney;

        public int CurrentMoney => _currentMoney;

        public void Initialize()
        {
            G.PlayerWallet = this;
            _currentMoney = 67;
        }
        
        public bool SpendMoney(int value)
        {
            if (_currentMoney < value)
            {
                Debug.LogWarning("Not enough money");
                return false;
            }
            
            _currentMoney -= value;
            
            AkatekuEventSystem.OnMoneyChanged.Invoke();
            return true;
        }

        public void IncreaseMoney(int value)
        {
            AkatekuEventSystem.OnMoneyChanged.Invoke();
            _currentMoney += value;
        }
    }
}