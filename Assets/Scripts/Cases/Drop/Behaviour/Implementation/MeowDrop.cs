using Cases.Drop.Behaviour;
using UnityEngine;

namespace LoxaRPG.Cases.Drop.Behaviour.Implementation
{
    /// <summary>
    /// Спавнит Акатеку67 — боевого компаньона.
    /// </summary>
    [CreateAssetMenu(fileName = "Meow Drop Behaviour", menuName = "Cases/Drop/Behaviour/Akateku67", order = 0)]
    public class MeowDrop : DropBehaviour
    {
        [SerializeField] private GameObject akateka67Prefab;
        [SerializeField] private float spawnHeight = 3f; // высота спавна над игроком

        public override void ApplyDrop()
        {
            SpawnAkateka67();
        }

        private void SpawnAkateka67()
        {
            if (akateka67Prefab == null)
            {
                Debug.LogError("MeowDrop: Префаб Акатеки67 не назначен!");
                return;
            }

            var player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogError("MeowDrop: Игрок не найден!");
                return;
            }

            var spawnPosition = player.transform.position + Vector3.up * spawnHeight;
            Instantiate(akateka67Prefab, spawnPosition, Quaternion.identity);

            Debug.Log("MeowDrop: Акатека67 появился!");
        }
    }
}