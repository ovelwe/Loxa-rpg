using Cases;
using Cases.Drop;
using UnityEngine;
using UnityEngine.UI;
using LoxaRPG.Player.Components;
using LoxaRPG.UI;

namespace LoxaRPG.Cases
{
    /// <summary>
    /// Контроллер открытия кейсов.
    /// </summary>
    public class CaseOpeningController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private CaseDropResolver dropResolver;
        [SerializeField] private CaseRouletteUI rouletteUI;
        [SerializeField] private Button openButton;
        [SerializeField] private PlayerWallet playerWallet;
        [SerializeField] private DropRewardUI dropRewardUI;

        [Header("Cost")]
        [SerializeField] private int caseCost = 67;

        [Header("Sounds")]
        [SerializeField] private AudioClip caseOpenSound; // звук открытия кейса

        private void Awake()
        {
            if (playerWallet == null)
                playerWallet = FindFirstObjectByType<PlayerWallet>();

            if (openButton != null)
                openButton.onClick.AddListener(OpenCase);
        }

        private void OnDestroy()
        {
            if (openButton != null)
                openButton.onClick.RemoveListener(OpenCase);
        }

        public void OpenCase()
        {
            if (rouletteUI == null || rouletteUI.IsSpinning) return;

            if (playerWallet == null)
            {
                Debug.LogError("CaseOpeningController: PlayerWallet не найден!");
                return;
            }

            if (!playerWallet.TrySpend(caseCost))
            {
                Debug.LogWarning("Недостаточно денег!");
                return;
            }

            // Звук открытия
            PlaySound(caseOpenSound);

            var winner = dropResolver.GetRandomDrop();

            if (openButton != null)
                openButton.interactable = false;

            rouletteUI.Spin(
                dropResolver.Drops,
                winner,
                () => OnSpinFinished(winner)
            );
        }

        private void OnSpinFinished(CaseDropData winner)
        {
            Debug.Log($"Выигрыш: {winner.itemName}");

            // Показываем награду
            if (dropRewardUI != null)
            {
                dropRewardUI.ShowWindow(winner);
            }
            else
            {
                Debug.LogError("DropRewardUI не назначен в CaseOpeningController!");
            }

            if (openButton != null)
                openButton.interactable = true;
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            if (LoxaRPG.Systems.SoundManager.Instance != null)
            {
                LoxaRPG.Systems.SoundManager.Instance.PlaySound(clip, transform.position);
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }
    }
}