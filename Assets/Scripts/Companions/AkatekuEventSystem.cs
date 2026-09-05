using UnityEngine.Events;

namespace Companions
{
    public static class AkatekuEventSystem
    {
        public static UnityEvent OnGameInitialized = new UnityEvent();
        
        public static UnityEvent OnMoneyChanged = new UnityEvent();
    }
}