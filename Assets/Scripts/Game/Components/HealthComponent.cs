using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthComponent : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public bool IsAlive => health > 0;
    [SerializeField] RagdollComponent _ragdollComponent;
    [SerializeField] LootDropComponent lootDropComponent;

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

        if (!gameObject.CompareTag("Player"))
        {
            if (_ragdollComponent)
            {
                StartCoroutine(DespawnTimer());
                WaveManager.CountDeadEnemies();
            }
            else
            {
                HandleEnemyPoolDeath();
                WaveManager.CountDeadEnemies();
            }
        }
        else
        {
            //Player Death Logic
            //Destroy(gameObject);
        }
        if (lootDropComponent != null)
        {
            lootDropComponent.DropLoot();
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

    public virtual IEnumerator DespawnTimer()
    {
       
        _ragdollComponent.EnableRagdoll();
        yield return new WaitForSeconds(1.3f);
        HandleEnemyPoolDeath();
        _ragdollComponent.DisableRagdoll();
    }

    public virtual void HandleEnemyPoolDeath()
    {
        ObjectPooler.EnemyPoolRelease(gameObject.tag, gameObject);
    }
}
