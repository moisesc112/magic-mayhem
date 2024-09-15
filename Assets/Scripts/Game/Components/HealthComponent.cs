using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float maxHealth;
    public float health;

    public virtual void Awake()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        Debug.Log($"took {damage} damage");
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            HandleDeath();
        }
    }

    public virtual void HandleDeath()
    {
        Debug.Log("Died");
        Destroy(gameObject);
    }

    public virtual void Heal(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
