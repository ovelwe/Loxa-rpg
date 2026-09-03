using Cases.Drop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cases
{
    public class CaseItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text itemNameText;

        public CaseDropData Data { get; private set; }

        public void Setup(CaseDropData data)
        {
            Data = data;

            if (icon != null)
                icon.sprite = data.itemSprite;

            if (itemNameText != null)
                itemNameText.text = data.itemName;
        }
    }
}