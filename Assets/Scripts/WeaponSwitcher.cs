using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSwitcher : MonoBehaviour
{
    public Weapon[] weapons;
    private int currentIndex = 0;

    void Start()
    {
        EquipWeapon(0);
    }

    void Update()
    {
        // Переключение оружия
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        // Стрельба только если не наведён на UI
        if (Input.GetMouseButton(0) && weapons[currentIndex] != null && !IsPointerOverUI())
        {
            weapons[currentIndex].TryShoot();
        }
    }

    // Проверка, находится ли курсор над UI элементом
    private bool IsPointerOverUI()
    {
        // Для мобильных устройств
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        
        // Для ПК
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].gameObject.SetActive(i == index);
            }
        }
        currentIndex = index;
    }
}