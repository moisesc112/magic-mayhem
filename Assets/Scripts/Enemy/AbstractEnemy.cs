using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(HealthComponent))]
public abstract class AbstractEnemy : MonoBehaviour
{
	//properties that will be used to reference
	private Player targetPlayer;
	private NavMeshAgent agent;
	public LayerMask isPlayer;

	// Public properties
	public float attackRange;
	public float attackDamage;
	public float attackCooldownTimer = 1.5f;
	public float movementSpeed;

	// Private bools
	private bool inAttackRange= false;
	private bool onCooldown = false;
	bool isAttacking = true;

	[SerializeField] DistancePollObject[] distancePollInfos;
	float _distancePollInterval = 0.1f;

	private void Awake()
	{
		// Set up the agent and target
		agent = GetComponent<NavMeshAgent>();
		agent.speed = movementSpeed;

		// Order infos based on distance threshold to make querying a bit faster.
		distancePollInfos = distancePollInfos.OrderBy(p => p.DistanceSqThreshold).ToArray();
	}

	void Start()
	{
		StartCoroutine(nameof(EnemyPolling));
	}

	void Update()
	{
		UpdatePollInfo();
	}

	void FixedUpdate()
	{
		// Check if enemy is in attack range to the player
		inAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

		// Attack or Find player
		if (inAttackRange) AttackPlayer();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 1, 0, 0.75F);
		Gizmos.DrawSphere(transform.position, attackRange);
		Gizmos.color = new Color(0, 0, 1, 0.75f);
		Gizmos.DrawWireSphere(agent.destination, 0.5f);
	}

	public void ResetPolling()
	{
		StopCoroutine(nameof(EnemyPolling));
		StartCoroutine(nameof(EnemyPolling));
	}

	public virtual void AttackPlayer()
	{
		// If in range don't move and face player
		transform.LookAt(targetPlayer.transform.position);

		// Get health of player and attack with cooldown
		HealthComponent targetHealth = targetPlayer.GetComponent<HealthComponent>();
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
		while (isAttacking)
		{
			// There is a chance that the coroutine could start before players are intialized. Wait a frame before processing.
			if (targetPlayer is null)
			{
				yield return new WaitForEndOfFrame();
			}
			else
            {
				agent.SetDestination(targetPlayer?.GetAvatarPosition() ?? Vector3.zero);
				yield return new WaitForSeconds(_distancePollInterval);
			}
		}
	}

	// Cooldown reset
	IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(attackCooldownTimer);
		onCooldown = false;
	}

	/// <summary>
	/// Grabs distance poll info accordeing to nearest player. In addition, sets target player.
	/// </summary>
	void UpdatePollInfo()
	{
		var newPollInterval = GetPollStateInfoFromDistance().PollInterval;
		if (_distancePollInterval != newPollInterval)
		{
			_distancePollInterval = newPollInterval;
			ResetPolling();
		}
	}

	DistancePollObject GetPollStateInfoFromDistance()
	{
		(var player, var distanceSq) = FindNearestPlayer();

		targetPlayer = player;

		DistancePollObject pollStateInfo = null;

		// Default to closest distance poll state
		foreach (var pollStat in distancePollInfos)
		{
			if (distanceSq > pollStat.DistanceSqThreshold) // Target is too far to use this poll info
			{
				continue;
			}
			else
			{
				pollStateInfo = pollStat;
				break;
			}
		}

		// If we are outside the range of all polls, use the furthest one.
		return pollStateInfo ?? distancePollInfos.Last();
	}


	(Player player, float distanceSq) FindNearestPlayer()
	{
		// For now have a base AI that always moves towards the player
		// Will need to have more advanced logic/ways to
		// distinguish when there are multiple players but
		// good enough for demo
		var players = FindObjectsOfType<Player>();
		var closestPlayer = players.FirstOrDefault();
		float closestDistance = float.MaxValue;
		foreach (var player in players)
		{
			var distSq = (player.GetAvatarPosition() - transform.position).sqrMagnitude;
			if (distSq < closestDistance)
			{
				closestPlayer = player;
				closestDistance = distSq;
			}
		}
		// If there are no targets found, stop nav-ing.
		if (closestDistance is float.MaxValue || closestPlayer is null)
			agent.destination = transform.position;

		return (closestPlayer, closestDistance);
	}
}