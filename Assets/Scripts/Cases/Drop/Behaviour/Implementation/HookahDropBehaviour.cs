using Cases.Drop.Behaviour;
using UnityEngine;
using LoxaRPG.Player.Components;

namespace LoxaRPG.Cases.Drop.Behaviour.Implementation
{
    /// <summary>
    /// Кальян. Лечит игрока на указанное количество ХП.
    /// </summary>
    [CreateAssetMenu(fileName = "Hookah Drop Behaviour", menuName = "Cases/Drop/Behaviour/Hookah", order = 0)]
    public class HookahDropBehaviour : DropBehaviour // БЫЛО DropBehavior — теперь правильно
    {
        [SerializeField] private int healAmount = 10;

        public override void ApplyDrop()
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogError("HookahDropBehaviour: Игрок не найден!");
                return;
            }

            if (!player.TryGetComponent<PlayerHealth>(out var health))
            {
                Debug.LogError("HookahDropBehaviour: PlayerHealth не найден на игроке!");
                return;
            }

            health.Heal(healAmount);
            Debug.Log($"HookahDropBehaviour: Игрок вылечен на {healAmount} HP");
        }
    }
}