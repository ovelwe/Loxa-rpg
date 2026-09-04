using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [Header("Настройки кулдаунов")]
    public float hurtSoundCooldown = 0.3f; // Кулдаун для звуков урона
    public float globalSoundCooldown = 0.1f; // Общий кулдаун
    
    private float lastHurtSoundTime = 0;
    private float lastGlobalSoundTime = 0;
    
    void Awake()
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
    
    // Для звуков урона (с кулдауном)
    public void PlayHurtSound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        
        // Проверяем кулдаун
        if (Time.time < lastHurtSoundTime + hurtSoundCooldown)
        {
            return; // Пропускаем звук
        }
        
        lastHurtSoundTime = Time.time;
        
        PlaySound(clip, position, volume);
    }
    
    // Обычный звук (с общим кулдауном)
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        
        GameObject soundObject = new GameObject("Sound");
        soundObject.transform.position = position;
        
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D звук
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        
        audioSource.Play();
        
        Destroy(soundObject, clip.length);
    }
    
    // Звук без кулдауна (для важных звуков)
    public void PlaySoundNoCooldown(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        
        GameObject soundObject = new GameObject("Sound");
        soundObject.transform.position = position;
        
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        
        audioSource.Play();
        
        Destroy(soundObject, clip.length);
    }
}