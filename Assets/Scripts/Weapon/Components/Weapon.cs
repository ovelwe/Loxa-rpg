using UnityEngine;
using LoxaRPG.Systems;

namespace LoxaRPG.Weapons.Components
{
    /// <summary>
    /// Оружие. Стреляет пулями и проигрывает звук выстрела.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [Header("Основное")]
        [SerializeField] private string weaponName = "Пистолет";
        [SerializeField] private float damage = 10f;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private int pelletsPerShot = 1;
        [SerializeField] private float spreadAngle = 0f;

        [Header("Стрельба")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;

        [Header("Звук")]
        [SerializeField] private AudioClip shootSound;
        [Range(0f, 5f)] [SerializeField] private float soundVolume = 1f;

        private float _nextFireTime;

        public float Damage => damage; // наружу только читать

        /// <summary>
        /// Увеличить урон оружия.
        /// </summary>
        public void IncreaseDamage(float multiplier)
        {
            damage *= multiplier;
            Debug.Log($"Weapon ({weaponName}): Урон увеличен до {damage}");
        }

        public void TryShoot()
        {
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + 1f / fireRate;

            Shoot();
        }

        private void Shoot()
        {
            // Создаём пули
            for (int i = 0; i < pelletsPerShot; i++)
            {
                float angle = Random.Range(-spreadAngle, spreadAngle);
                var rotation = firePoint.rotation * Quaternion.Euler(0, 0, angle);

                var bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
                if (bullet.TryGetComponent<Bullet>(out var bulletScript))
                {
                    bulletScript.Init(damage, bulletSpeed);
                }
            }

            PlayShootSound();
        }

        private void PlayShootSound()
        {
            if (shootSound == null) return;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(shootSound, firePoint.position, soundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(shootSound, firePoint.position);
            }
        }
    }
}