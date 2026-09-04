using UnityEngine;

public class CaseSystem : MonoBehaviour
{
    public CaseItem[] items;
    public int caseCost = 67;
    public GameObject caseOpenPanel;
    
    [Header("Акатека67")]
    public GameObject akateka67Prefab;
    
    [Header("Звук открытия кейса")]
    public AudioClip caseOpenSound; // Звук открытия кейса
    [Range(0f, 5f)] public float caseOpenSoundVolume = 2f;
    
    public void BuyCase()
    {
        if (GameManager.Instance != null && GameManager.Instance.money >= caseCost)
        {
            GameManager.Instance.SpendMoney(caseCost);
            Debug.Log("Куплен кейс за " + caseCost + " монет");
            
            // Проигрываем звук открытия кейса
            PlayCaseOpenSound();
            
            OpenCase();
        }
        else
        {
            Debug.Log("Недостаточно монет! Нужно " + caseCost + ", у тебя " + 
                     (GameManager.Instance != null ? GameManager.Instance.money : 0));
        }
    }

    public void OpenCase()
    {
        if (items == null || items.Length == 0)
        {
            Debug.LogError("Нет предметов в кейсе! Добавь их в инспекторе.");
            return;
        }

        float totalWeight = 0;
        foreach (var item in items) totalWeight += item.dropChance;

        float random = Random.Range(0, totalWeight);
        float cumulative = 0;
        CaseItem droppedItem = items[0];

        foreach (var item in items)
        {
            cumulative += item.dropChance;
            if (random <= cumulative)
            {
                droppedItem = item;
                break;
            }
        }

        // ПРОИГРЫВАЕМ ЗВУК ВЫПАВШЕГО ПРЕДМЕТА
        PlayItemDropSound(droppedItem);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowCaseReward(droppedItem);
        }
        else
        {
            Debug.LogError("UIManager.Instance is null!");
        }
        
        ApplyItem(droppedItem);
    }

    void PlayCaseOpenSound()
    {
        if (caseOpenSound == null) return;
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySoundNoCooldown(caseOpenSound, transform.position, caseOpenSoundVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(caseOpenSound, transform.position);
        }
    }
    
    void PlayItemDropSound(CaseItem item)
    {
        if (item.dropSound == null) return;
        
        // Задержка, чтобы звук выпадения не накладывался на звук открытия
        StartCoroutine(PlayDropSoundDelayed(item));
    }
    
    System.Collections.IEnumerator PlayDropSoundDelayed(CaseItem item)
    {
        // Ждём немного после открытия кейса
        yield return new WaitForSeconds(0.5f);
        
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySoundNoCooldown(item.dropSound, transform.position, item.dropSoundVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(item.dropSound, transform.position);
        }
    }

    void ApplyItem(CaseItem item)
    {
        Debug.Log("Выпал предмет: " + item.itemName);
        
        switch (item.itemName)
        {
            case "Монеты +100":
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddMoney(100);
                }
                break;
                
            case "Урон +10%":
                Weapon[] weapons = FindObjectsOfType<Weapon>();
                foreach (Weapon weapon in weapons)
                {
                    weapon.damage *= 1.1f;
                }
                break;
                
            case "Акатека67":
                SpawnAkateka67();
                break;
                
            case "Скин золотой":
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.color = Color.yellow;
                    }
                }
                break;
                
            case "Кальян":
                HealPlayer(10);
                break;
        }
    }
    
    void SpawnAkateka67()
    {
        if (akateka67Prefab == null)
        {
            Debug.LogError("Акатека67: Префаб не назначен в CaseSystem!");
            return;
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Акатека67: Игрок не найден!");
            return;
        }
        
        Vector3 spawnPosition = player.transform.position + Vector3.up * 3f;
        GameObject akateka = Instantiate(akateka67Prefab, spawnPosition, Quaternion.identity);
        
        Debug.Log("Акатека67 появился!");
    }
    
    void HealPlayer(int healAmount)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Кальян: Игрок не найден!");
            return;
        }
        
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            playerHealth.PlayHealEffect();
            Debug.Log("Кальян выкурен! +" + healAmount + " HP!");
        }
    }
}