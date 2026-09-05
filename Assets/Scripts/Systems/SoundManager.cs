using UnityEngine;

namespace LoxaRPG.Systems
{
    /// <summary>
    /// Проигрывает звуки с кулдаунами, чтобы не было каши.
    /// Синглтон, живёт на сцене и не умирает.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Кулдауны")]
        [SerializeField] private float hurtSoundCooldown = 0.3f; // задержка между звуками урона

        private float _lastHurtSoundTime;

        private void Awake()
        {
            // Классический синглтон. Если уже есть — уничтожаем дубликат.
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // живёт вечно
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Звук урона. Имеет кулдаун, чтобы не спамить уши.
        /// </summary>
        public void PlayHurtSound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;

            // Если кулдаун не прошёл — пропускаем звук.
            if (Time.time < _lastHurtSoundTime + hurtSoundCooldown)
                return;

            _lastHurtSoundTime = Time.time;
            PlaySound(clip, position, volume);
        }

        /// <summary>
        /// Обычный звук. Без кулдауна, просто играет.
        /// </summary>
        public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;

            var soundObject = new GameObject("Sound");
            soundObject.transform.position = position;

            var audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = 0f; // 2D звук, слышно везде одинаково
            audioSource.pitch = Random.Range(0.9f, 1.1f); // лёгкий разброс тона

            audioSource.Play();
            Destroy(soundObject, clip.length); // убиваем объект после проигрывания
        }
    }
}