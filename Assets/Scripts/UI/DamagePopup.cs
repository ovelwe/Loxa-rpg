using UnityEngine;

namespace LoxaRPG.UI
{
    /// <summary>
    /// Показывает всплывающий урон над врагами.
    /// </summary>
    public class DamagePopup : MonoBehaviour
    {
        public static DamagePopup Instance { get; private set; }

        [SerializeField] private GameObject damageTextPrefab;
        [SerializeField] private float lifeTime = 1f;
        [SerializeField] private float moveSpeed = 2f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ShowDamage(Vector3 position, float damageAmount)
        {
            if (damageTextPrefab == null)
            {
                Debug.LogError("DamagePopup: Префаб текста урона не назначен!");
                return;
            }

            var damageObj = Instantiate(damageTextPrefab, position, Quaternion.identity);

            if (!damageObj.TryGetComponent<DamageTextAnimation>(out var anim))
            {
                anim = damageObj.AddComponent<DamageTextAnimation>();
            }

            anim.Setup(damageAmount, lifeTime, moveSpeed);
        }
    }
}