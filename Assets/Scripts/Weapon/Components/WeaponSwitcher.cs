using UnityEngine;
using UnityEngine.EventSystems;

namespace LoxaRPG.Weapons.Components
{
    /// <summary>
    /// Переключает оружие и стреляет.
    /// </summary>
    public class WeaponSwitcher : MonoBehaviour
    {
        [SerializeField] private Weapon[] weapons;

        private int _currentIndex;

        private void Start()
        {
            EquipWeapon(0);
        }

        private void Update()
        {
            // Переключение по клавишам
            if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

            // Стрельба, если не наведён на UI
            if (Input.GetMouseButton(0) && weapons[_currentIndex] != null && !IsPointerOverUI())
            {
                weapons[_currentIndex].TryShoot();
            }
        }

        private void EquipWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length) return;

            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                    weapons[i].gameObject.SetActive(i == index);
            }

            _currentIndex = index;
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}