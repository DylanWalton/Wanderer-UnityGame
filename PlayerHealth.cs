using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    int health;
    PlayerStats stats;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        health = stats.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

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
    }

    void Die()
    {
        Debug.Log(name + " has died!");
        
        // Die
    }
}
