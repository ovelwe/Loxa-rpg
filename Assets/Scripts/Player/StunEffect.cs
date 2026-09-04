using UnityEngine;
using System.Collections;

public class StunEffect : MonoBehaviour
{
    [Header("Настройки стана")]
    public float stunDuration = 1f;
    public GameObject stunVisual;
    
    private bool isStunned = false;
    private MonoBehaviour[] playerScripts; // Все скрипты игрока
    
    void Start()
    {
        // Получаем все MonoBehaviour скрипты на игроке
        playerScripts = GetComponents<MonoBehaviour>();
    }
    
    public void StunPlayer(float duration = -1f)
    {
        if (duration > 0)
        {
            stunDuration = duration;
        }
        
        if (!isStunned)
        {
            StartCoroutine(StunCoroutine());
        }
    }
    
    IEnumerator StunCoroutine()
    {
        isStunned = true;
        Debug.Log("Игрок оглушён на " + stunDuration + " секунд!");
        
        // Отключаем ВСЕ скрипты на игроке, кроме StunEffect и PlayerHealth
        foreach (MonoBehaviour script in playerScripts)
        {
            if (script != null && script != this && script.GetType() != typeof(PlayerHealth))
            {
                script.enabled = false;
            }
        }
        
        // Показываем визуал стана
        if (stunVisual != null)
        {
            stunVisual.SetActive(true);
        }
        
        // Ждём
        yield return new WaitForSeconds(stunDuration);
        
        // Включаем скрипты обратно
        foreach (MonoBehaviour script in playerScripts)
        {
            if (script != null && script != this && script.GetType() != typeof(PlayerHealth))
            {
                script.enabled = true;
            }
        }
        
        // Скрываем визуал
        if (stunVisual != null)
        {
            stunVisual.SetActive(false);
        }
        
        isStunned = false;
        Debug.Log("Игрок пришёл в себя!");
    }
}