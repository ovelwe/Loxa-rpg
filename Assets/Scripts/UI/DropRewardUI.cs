using Cases;
using Cases.Drop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DropRewardUI : MonoBehaviour
    {
        [SerializeField] private RectTransform container;
        
        [Space(5)]
        [SerializeField] private Image dropImage;
        [SerializeField] private TMP_Text dropNameText;
        [SerializeField] private Button claimButton;

        private void Start()
        {
            G.DropRewardUI = this;
        }

        private void OnDestroy()
        {
            G.DropRewardUI = this;
        }

        public void ShowWindow(CaseDropData dropData)
        {
            dropImage.sprite = dropData.itemSprite;
            dropNameText.text = dropData.itemName;
            
            container.gameObject.SetActive(true);
            
            AudioSource.PlayClipAtPoint(dropData.dropSound, Vector3.zero);
            
            claimButton.onClick.AddListener(() =>
            {
                dropData.ApplyDrop();
                HideWindow();
            });
        }

        private void HideWindow()
        {
            container.gameObject.SetActive(false);
        }
    }
}