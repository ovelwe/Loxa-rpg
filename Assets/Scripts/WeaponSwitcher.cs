using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        if (Input.GetMouseButton(0) && weapons[currentIndex] != null)
        {
            weapons[currentIndex].TryShoot();
        }
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == index);
        }
        currentIndex = index;
    }
}