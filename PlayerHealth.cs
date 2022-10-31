using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    float health;
    float prevHealth;
    PlayerStats stats;

    public Image healthBarFill;
    public Image healthBarEffect;
    public float effectLerpSpeed = .5f;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        health = stats.maxHealth;
        prevHealth = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        prevHealth = healthBarEffect.fillAmount;

        // Health Bar Stuff
        healthBarFill.fillAmount = health/stats.maxHealth;
        StartCoroutine(HealthBarEffect());

        if (health <= 0) {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (health + amount > stats.maxHealth)
            health = stats.maxHealth;
        else
            health += amount;

        // Health Bar Stuff
        healthBarFill.fillAmount = health / stats.maxHealth;
    }

    IEnumerator HealthBarEffect()
    {
        while (healthBarEffect.fillAmount > healthBarFill.fillAmount)
        {
            healthBarEffect.fillAmount -= effectLerpSpeed*Time.deltaTime;

            yield return null;
        }
    }

    void Die()
    {
        Debug.Log(name + " has died!");
        
        // Die
    }
}
