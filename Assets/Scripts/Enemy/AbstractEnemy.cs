using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthComponent))]
public abstract class AbstractEnemy : MonoBehaviour
{
    public GameObject target;
    private NavMeshAgent agent;
    public LayerMask isPlayer;

    // Public properties
    public float attackRange;
    public float attackDamage;
    public float attackCooldownTimer;
    public float movementSpeed;

    // Private bools
    private bool inAttackRange= false;
    private bool onCooldown = false;

    private void Awake()
    {
        // Set up the agent and target
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player");
        agent.speed = movementSpeed;
    }

    void Update()
    {
        // Check if enemy is in attack range to the player
        inAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

        // Attack or find player
        if (inAttackRange)
        {
            AttackPlayer();
            //Debug.Log("Enemy in range");
        }
        else
        {
            FindPlayer();
        }

    }

    public virtual void FindPlayer()
    {
        // For now have a base AI to always move towards the player
        // Will need to have more advanced logic when there are 
        // multiple players but good enough for demo
        agent.destination = target.transform.position;
    }

    public virtual void AttackPlayer()
    {
        
        // If in range don't move and face player
        agent.SetDestination(target.transform.position);
        transform.LookAt(target.transform);

        // Get health of player and attack with cooldown
        HealthComponent targetHealth = target.GetComponent<HealthComponent>();
        if (targetHealth != null && !onCooldown)
        {
            Debug.Log("Enemy attacking player!");
            targetHealth.TakeDamage(attackDamage);
            Debug.Log(targetHealth.health);
            onCooldown = true;
            StartCoroutine(AttackCooldown());
        }
    }

    // Cooldown reset
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldownTimer);
        onCooldown = false;
    }

}