using UnityEngine;

namespace Cases.Drop.Behaviour.Implementation
{
    [CreateAssetMenu(fileName = "Damage Drop Behaviour", menuName = "Cases/Drop/Behaviour/Damage", order = 0)]
    public class DamageDropBehaviour : DropBehaviour
    {
        public override void ApplyDrop()
        {
            //todo: нахуй пиздец.
            Weapon[] weapons = FindObjectsOfType<Weapon>();
            foreach (Weapon weapon in weapons)
            {
                weapon.damage *= 1.1f;
            }
        }
    }
}