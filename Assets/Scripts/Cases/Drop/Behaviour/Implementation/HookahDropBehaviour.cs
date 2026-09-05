using UnityEngine;

namespace Cases.Drop.Behaviour.Implementation
{
    [CreateAssetMenu(fileName = "Hookah Drop Behaviour", menuName = "Cases/Drop/Behaviour/Hookah", order = 0)]
    public class HookahDropBehaviour : DropBehaviour
    {
        [field: SerializeField] public int healAmount;
        
        public override void ApplyDrop()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Кальян: Игрок не найден!");
                return;
            }
        
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                playerHealth.PlayHealEffect();
                Debug.Log("Кальян выкурен! +" + healAmount + " HP!");
            }
        }
    }
}