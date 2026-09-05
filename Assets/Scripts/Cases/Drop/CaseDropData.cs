using Cases.Drop.Behaviour;
using UnityEngine;

namespace Cases.Drop
{
    [CreateAssetMenu(fileName = "CaseDropData", menuName = "Cases/New Drop Data", order = 0)]
    public class CaseDropData : ScriptableObject
    {
        [field: SerializeField] public string itemName;
        [field: SerializeField, Range(0f, 1f)] public float itemChance;
        [field: SerializeField] public Sprite itemSprite;
        
        [field: SerializeField] public AudioClip dropSound;
        
        [field: SerializeField] public DropBehaviour dropBehaviour;
        
        public void ApplyDrop()
        {
            if (dropBehaviour == null)
            {
                Debug.LogWarning(
                    $"DropBehaviour not assigned for {itemName}"
                );

                return;
            }

            dropBehaviour.ApplyDrop();
        }
    }
}