using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    float health;
    
    [Header("Damage Effect")]
    public DamageEffect damageEffect;
    public float damageEffectDuration;
    
    public void TakeDamage(float damage)
    {
        health -= damage;


        if (health < 0)
        {
            Destroy(gameObject);
        }

        if (damageEffect != null)
        {
            StartCoroutine(damageEffect.Effect(damageEffectDuration));
        }
    }
    
    void Awake()
    {
        health = maxHealth;
    }
}
