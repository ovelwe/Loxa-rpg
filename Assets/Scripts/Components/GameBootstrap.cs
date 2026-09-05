using Player.Wallet;
using UnityEngine;

namespace Components
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private PlayerWallet playerWallet;
        
        private void Start()
        {
            G.PlayerTransform = playerTransform;
            playerWallet.Initialize();
            
            AkatekuEventSystem.OnGameInitialized?.Invoke();
        }

        private void OnDestroy()
        {
            G.PlayerTransform = null;
        }
    }
}