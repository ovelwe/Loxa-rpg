using Cases.Drop.Behaviour;
using UnityEngine;
using LoxaRPG.Weapons.Components;

namespace LoxaRPG.Cases.Drop.Behaviour.Implementation
{
    /// <summary>
    /// Увеличивает урон всего оружия на 10%.
    /// </summary>
    [CreateAssetMenu(fileName = "Damage Drop Behaviour", menuName = "Cases/Drop/Behaviour/Damage", order = 0)]
    public class DamageDropBehaviour : DropBehaviour
    {
        [SerializeField] private float damageMultiplier = 1.1f; // насколько увеличиваем

        public override void ApplyDrop()
        {
            // Находим всё оружие и увеличиваем урон
            var weapons = FindObjectsOfType<Weapon>();

            if (weapons.Length == 0)
            {
                Debug.LogWarning("DamageDropBehaviour: Оружие не найдено!");
                return;
            }

            foreach (var weapon in weapons)
            {
                weapon.IncreaseDamage(damageMultiplier);
            }

            Debug.Log($"DamageDropBehaviour: Урон оружия увеличен на {(damageMultiplier - 1f) * 100f}%");
        }
    }
}