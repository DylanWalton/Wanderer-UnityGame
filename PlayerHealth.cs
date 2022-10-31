using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    float health;
    PlayerStats stats;

    public Image healthBarFill;
    public Image healthBarEffect;
    public float effectLerpSpeed;
    //public GameObject healthText;
    //TextMeshPro m_HealthText;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        health = stats.maxHealth;
        //m_HealthText = healthText.GetComponent<TextMeshPro>();
        //m_HealthText.text = stats.maxHealth + " / " + stats.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Health Bar Stuff
        healthBarFill.fillAmount = health/stats.maxHealth;
        while (healthBarEffect.fillAmount > healthBarFill.fillAmount)
        {
            healthBarEffect.fillAmount = Mathf.Lerp(healthBarEffect.fillAmount, healthBarFill.fillAmount, effectLerpSpeed);
        }

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
        //while (healthBarEffect.fillAmount > healthBarFill.fillAmount)
        //{
        //    healthBarEffect.fillAmount = Mathf.Lerp(healthBarEffect.fillAmount, healthBarFill.fillAmount, effectLerpSpeed);
        //}
    }

    void Die()
    {
        Debug.Log(name + " has died!");
        
        // Die
    }
}
