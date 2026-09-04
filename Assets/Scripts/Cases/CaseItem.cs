using UnityEngine;

[System.Serializable]
public class CaseItem
{
    public string itemName;
    public Sprite icon;
    [Range(0f, 100f)] public float dropChance;
    public AudioClip dropSound; // Звук выпадения предмета
    [Range(0f, 5f)] public float dropSoundVolume = 2f; // Громкость звука
}