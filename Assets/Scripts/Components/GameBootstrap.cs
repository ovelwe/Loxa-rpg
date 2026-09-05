using Companions;
using Player.Wallet;
using UnityEngine;

namespace Components
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerWallet playerWallet;
        
        private void Start()
        {
            playerWallet.Initialize();
            
            AkatekuEventSystem.OnGameInitialized?.Invoke();
        }
    }
}