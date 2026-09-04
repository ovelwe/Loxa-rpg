using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName = "Пистолет";
    public float damage = 10f;
    public float fireRate = 0.5f; // выстрелов в секунду
    public float bulletSpeed = 20f;
    public int pelletsPerShot = 1; // для дробовика
    public float spreadAngle = 0f; // разброс в градусах

    public GameObject bulletPrefab;
    public Transform firePoint;
    public AudioClip shootSound;

    private float nextFireTime;

    public void TryShoot()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    protected virtual void Shoot()
    {
        for (int i = 0; i < pelletsPerShot; i++)
        {
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angle);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Init(damage, bulletSpeed);
            }
        }
        if (shootSound != null) AudioSource.PlayClipAtPoint(shootSound, firePoint.position);
    }
}