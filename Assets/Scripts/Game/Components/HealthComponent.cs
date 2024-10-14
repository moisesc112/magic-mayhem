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
		_dissolver = GetComponent<Dissolver>();
	}

<<<<<<< HEAD
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
=======
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
>>>>>>> b1a041f37349cc6856af0ad78da76a10ba364289

    public virtual void HandleDeath()
    {
        Debug.Log("Died");

        if (!gameObject.CompareTag("Player"))
        {
<<<<<<< HEAD
            if (_ragdollComponent && _dissolver)
            {
                StartCoroutine(DespawnTimer());
            }
            else // Test Enemy doesn't have a rag doll, so don't need to start a despawn timer for it
            {
                HandleEnemyPoolDeath();
            }
            WaveManager.CountDeadEnemies();
=======
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
>>>>>>> b1a041f37349cc6856af0ad78da76a10ba364289
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
<<<<<<< HEAD
        _dissolver.StartDissolving();
        yield return new WaitForSeconds(2f);
        HandleEnemyPoolDeath();
        _ragdollComponent.DisableRagdoll();
        _dissolver.ResetEffect();
=======
        yield return new WaitForSeconds(1.3f);
        HandleEnemyPoolDeath();
        _ragdollComponent.DisableRagdoll();
>>>>>>> b1a041f37349cc6856af0ad78da76a10ba364289
    }

    public virtual void HandleEnemyPoolDeath()
    {
        ObjectPooler.EnemyPoolRelease(gameObject.tag, gameObject);
    }
<<<<<<< HEAD

    Dissolver _dissolver;
=======
>>>>>>> b1a041f37349cc6856af0ad78da76a10ba364289
}
