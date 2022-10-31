using UnityEngine;

public class Health : MonoBehaviour
{
    public float health;
    public Animator anim;
    public Rigidbody2D rb;
    public BoxCollider2D clder;

    [Header("Damage Effect")]
    public DamageEffect damageEffect;
    public float damageEffectDuration;
    
    public void TakeDamage(int damage)
    {
        health -= damage;


        if (health <= 0 && anim != null)
        {
            Destroy(clder);
            anim.SetTrigger("Death");
            Destroy(rb);
        }
        else if (health <= 0)
            Destroy(gameObject);

        if (damageEffect != null)
        {
            StartCoroutine(damageEffect.Effect(damageEffectDuration));
        }
        else if (anim != null)
            anim.SetTrigger("Hurt");
    }
}
