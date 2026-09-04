using UnityEngine;
using System.Collections;

public class ShockwaveAnimation : MonoBehaviour
{
    private float maxScale;
    private float duration;
    private bool isAnimating = false;
    
    public void Initialize(float maxSize, float animDuration)
    {
        maxScale = maxSize;
        duration = animDuration;
    }
    
    public void StartAnimation()
    {
        if (!isAnimating)
        {
            StartCoroutine(Animate());
        }
    }
    
    IEnumerator Animate()
    {
        isAnimating = true;
        float timer = 0;
        
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * maxScale;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = Mathf.Lerp(0.8f, 0f, t);
                renderer.color = color;
            }
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
}