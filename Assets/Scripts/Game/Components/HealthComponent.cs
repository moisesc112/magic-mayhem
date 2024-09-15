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

    // To work with the different instances from the object pool, need to release test enemy in its own class

    public virtual void HandleDeath()
    {
        Debug.Log("Died");
        if (gameObject.CompareTag("TestEnemyPool"))
        {
            TestEnemy testEnemy = GetComponent<TestEnemy>();
            testEnemy.TestEnemyPoolRelease();
            TestEnemy.CountDeadTestEnemies();
        }
        else
        {
            Destroy(gameObject);
        }
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
