using Cases.Drop;
using UnityEngine;
using UnityEngine.UI;

namespace Cases
{
    public class CaseOpeningController : MonoBehaviour
    {
        [SerializeField] private CaseDropResolver dropResolver;
        [SerializeField] private CaseRouletteUI rouletteUI;
        [SerializeField] private Button openButton;

        private void Awake()
        {
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
            if (rouletteUI.IsSpinning)
                return;

            // ВАЖНО:
            // результат определяется ДО запуска анимации.
            CaseDropData winner = dropResolver.GetRandomDrop();

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
            Debug.Log($"You won: {winner.itemName}");

            // Здесь:
            // Inventory.Add(winner);
            // Save();
            // показать окно победы и т.д.
            
            G.DropRewardUI.ShowWindow(winner);
            winner.ApplyDrop();

            if (openButton != null)
                openButton.interactable = true;
        }
    }
}