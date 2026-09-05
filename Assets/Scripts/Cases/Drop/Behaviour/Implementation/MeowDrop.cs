using UnityEngine;

namespace Cases.Drop.Behaviour.Implementation
{
    [CreateAssetMenu(fileName = "Meow Drop Behaviour", menuName = "Cases/Drop/Behaviour/Akateku67", order = 0)]
    public class MeowDrop : DropBehaviour
    {
        [field: SerializeField] public GameObject akateka67Prefab;
        
        public override void ApplyDrop()
        {
            SpawnAkateka67();
        }
        
        private void SpawnAkateka67()
        {
            if (akateka67Prefab == null)
            {
                Debug.LogError("Акатека67: Префаб не назначен в CaseSystem!");
                return;
            }
        
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Акатека67: Игрок не найден!");
                return;
            }
        
            Vector3 spawnPosition = player.transform.position + Vector3.up * 3f;
            GameObject _ = Instantiate(akateka67Prefab, spawnPosition, Quaternion.identity);
        
            Debug.Log("Акатека67 появился!");
        }
    }
}