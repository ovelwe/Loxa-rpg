using UnityEngine;

namespace Cases.Drop.Behaviour.Implementation
{
    [CreateAssetMenu(fileName = "Meow Drop Behaviour", menuName = "Cases/Drop/Behaviour", order = 0)]
    public class MeowDrop : DropBehaviour
    {
        public override void ApplyDrop()
        {
            Debug.LogError("meooooow");
        }
    }
}