using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName = "Пистолет";
    public float damage = 10f;
    public float fireRate = 0.5f; // выстрелов в секунду
    public float bulletSpeed = 20f;
    public int pelletsPerShot = 1; // для дробовика
    public float spreadAngle = 0f; // разброс в градусах

    [Header("Настройки стрельбы")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    [Header("Звук выстрела")]
    public AudioClip shootSound;
    [Range(0f, 5f)] public float shootSoundVolume = 1f; // Громкость выстрела
    [Range(0.5f, 2f)] public float shootPitchMin = 0.9f; // Минимальный тон
    [Range(0.5f, 2f)] public float shootPitchMax = 1.1f; // Максимальный тон

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
        // Создаём пули
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
        
        // Проигрываем звук выстрела
        PlayShootSound();
    }
    
    void PlayShootSound()
    {
        if (shootSound == null) return;
        
        if (SoundManager.Instance != null)
        {
            // Используем SoundManager (звук без кулдауна, чтобы каждый выстрел был слышен)
            SoundManager.Instance.PlaySoundNoCooldown(
                shootSound, 
                firePoint.position, 
                shootSoundVolume
            );
        }
        else
        {
            // Если SoundManager нет — проигрываем напрямую с громкостью
            GameObject soundObject = new GameObject("ShootSound");
            soundObject.transform.position = firePoint.position;
            
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = shootSound;
            audioSource.volume = shootSoundVolume;
            audioSource.spatialBlend = 0f; // 2D звук
            audioSource.pitch = Random.Range(shootPitchMin, shootPitchMax); // Разброс тона
            
            audioSource.Play();
            
            Destroy(soundObject, shootSound.length);
        }
    }
}