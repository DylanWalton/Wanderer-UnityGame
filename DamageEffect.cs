using System.Collections;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite damageSprite;

    public IEnumerator Effect(float duration)
    {
        float elapsed = .0f;

        Sprite originalSprite = spriteRenderer.sprite;

        while (elapsed < duration)
        {
            spriteRenderer.sprite = damageSprite;
            
            elapsed += Time.deltaTime;

            yield return null;
        }

        spriteRenderer.sprite = originalSprite;
    }
}
