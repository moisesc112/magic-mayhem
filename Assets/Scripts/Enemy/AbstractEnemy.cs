using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthComponent))]
public abstract class AbstractEnemy : MonoBehaviour
{
    //properties that will be used to reference
    private GameObject target;
    private NavMeshAgent agent;
    public LayerMask isPlayer;

    // Public properties
    public float attackRange;
    public float attackDamage;
    public float attackCooldownTimer = 1.5f;
    public float movementSpeed;
    public float closePollingInterval = 0.1f;
    public float farPollingInterval = 1f;
    public float farDistance = 10f;

    // Private bools
    private bool inAttackRange= false;
    private bool onCooldown = false;
    private float pollInterval;
    private float sqDistance;

    private void Awake()
    {
        // Set up the agent and target
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
    }

    void FixedUpdate()
    {
        // Check if enemy is in attack range to the player
        inAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

        // Attack or Find player
        if (inAttackRange)
        {
            AttackPlayer();
        }
        else
        {
            FindPlayer();
        }

    }

    public virtual void FindPlayer()
    {
        // For now have a base AI to always move towards the player
        // Will need to have more advanced logic/ways to
        // distinguish when there are multiple players but
        // good enough for demo
        target = GameObject.FindGameObjectWithTag("Player");
        agent.destination = target.transform.position;
        StartCoroutine(EnemyPolling());
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
            onCooldown = true;
            StartCoroutine(AttackCooldown());
        }
    }

    // Enemy Polling
    IEnumerator EnemyPolling()
    {
        // Use the squared distance to determine what poll inverval we should be using
        sqDistance = (target.transform.position - transform.position).sqrMagnitude;
        if (sqDistance < farDistance*farDistance)
        {
            pollInterval = closePollingInterval;
        }
        else
        {
            pollInterval = farPollingInterval;
        }
        yield return new WaitForSeconds(farPollingInterval);
    }

    // Cooldown reset
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldownTimer);
        onCooldown = false;
    }
}