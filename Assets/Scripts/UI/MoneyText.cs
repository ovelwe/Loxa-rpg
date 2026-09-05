using Companions;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class MoneyText : MonoBehaviour
    {
        private TMP_Text _text;
        
        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            
            AkatekuEventSystem.OnGameInitialized?.AddListener(() =>
            {
                _text.text = G.PlayerWallet.CurrentMoney.ToString();
            });
            AkatekuEventSystem.OnMoneyChanged?.AddListener(OnMoneyChanged);

        }

        private void OnDestroy()
        {
            AkatekuEventSystem.OnGameInitialized?.RemoveListener(() =>
            {
                _text.text = G.PlayerWallet.CurrentMoney.ToString();
            });
        }

        private void OnMoneyChanged()
        {
            _text.text = G.PlayerWallet.CurrentMoney.ToString();
        }
    }
}