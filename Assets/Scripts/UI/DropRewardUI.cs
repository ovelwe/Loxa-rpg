using Cases.Drop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LoxaRPG.Cases.Drop;
using LoxaRPG.Systems;

namespace LoxaRPG.UI
{
    /// <summary>
    /// Окно награды за кейс.
    /// </summary>
    public class DropRewardUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform container;
        [SerializeField] private Image dropImage;
        [SerializeField] private TMP_Text dropNameText;
        [SerializeField] private Button claimButton;

        [Header("Звук")]
        [SerializeField] private AudioClip rewardSound; // звук выпадения награды

        private CaseDropData _currentDrop;

        private void Awake()
        {
            if (container != null)
                container.gameObject.SetActive(false);

            if (claimButton != null)
                claimButton.onClick.AddListener(Claim);
        }

        private void OnDestroy()
        {
            if (claimButton != null)
                claimButton.onClick.RemoveListener(Claim);
        }

        public void ShowWindow(CaseDropData dropData)
        {
            _currentDrop = dropData;

            if (dropImage != null)
                dropImage.sprite = dropData.itemSprite;

            if (dropNameText != null)
                dropNameText.text = dropData.itemName;

            if (container != null)
                container.gameObject.SetActive(true);

            // Звук награды
            PlaySound(dropData.dropSound != null ? dropData.dropSound : rewardSound);
        }

        private void Claim()
        {
            if (_currentDrop != null)
            {
                _currentDrop.ApplyDrop();
            }

            HideWindow();
        }

        private void HideWindow()
        {
            if (container != null)
                container.gameObject.SetActive(false);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(clip, Vector3.zero);
            }
            else
            {
                AudioSource.PlayClipAtPoint(clip, Vector3.zero);
            }
        }
    }
}