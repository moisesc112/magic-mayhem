using UnityEngine;

public abstract class AbstractEntity : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public float movespeed;
    public float armor;

    public virtual void Awake()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        var armorReducedDamage = damage * GetPercentDamageTakenWithArmor(armor);
        health -= armorReducedDamage;
        if (health <= 0)
        {
            health = 0;
            HandleDeath();
        }
    }

    public virtual void HandleDeath() { }

    public virtual void Heal(float healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    
    public virtual float GetPercentDamageTakenWithArmor(float armor)
    {
        return 1 / (1 + (armor / 15));
    }
}
