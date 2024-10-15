using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityRandom = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(RagdollComponent))]
[RequireComponent(typeof(LootDropComponent))]
public class WarChief : MonoBehaviour
{
	[Header("Attack")]
	[SerializeField] float _meleeRange;
	[SerializeField] float _jumpRangeStart = 6.0f;
	[SerializeField] float _jumpRangeEnd = 12.0f;
	[SerializeField] float _damage;
	[SerializeField] RadialDamageSource _jumpSource;
	[SerializeField] Transform _hammerHitPoint;

	[Header("FX")] 
	[SerializeField] Renderer _renderer;


	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();
		_navPoller = GetComponent<NavPollerComponent>();
		_agent = GetComponent<NavMeshAgent>();
		_healthComp = GetComponent<HealthComponent>();
		_healthComp.onDeath += HealthComp_OnDeath;
		_dissolver = GetComponent<Dissolver>();
		_dissolver.SetTargetRenderer(_renderer);
		_ragdoll = GetComponent<RagdollComponent>();
		_lootDrop = GetComponent<LootDropComponent>();
	}

	void Start()
	{
		_swingLayerIndex = _animator.GetLayerIndex("Combat");
	}

	private void OnDestroy()
	{
		_healthComp.onDeath -= HealthComp_OnDeath;
	}

	private void FixedUpdate()
	{
		var distanceToPlayer = _navPoller.DistanceToPlayer;
		// Check if in jumping sweet spot.
		if (distanceToPlayer >= _jumpRangeStart && distanceToPlayer <= _jumpRangeEnd)
		{
			if (_canJump && !_isSwinging)
				JumpAttack();
		}

		if (distanceToPlayer <= _meleeRange && !_isJumping)
		{
			transform.LookAt(_navPoller.TargetPlayer?.GetAvatarPosition() ?? Vector3.zero);
			if (!_isSwinging)
				MeleeAttack();
		}
	}

	public void InitFromPool(Vector3 location, Action<WarChief> releaseAction)
	{
		transform.position = location;
		_agent.transform.position = transform.position;

		enabled = true;
		_navPoller.enabled = true;
		_agent.enabled = true;
		
		_canJump = true;

		_dissolver.ResetEffect();

		_healthComp.health = _healthComp.maxHealth;
		_ragdoll.DisableRagdoll();

		if (_releaseToPoolAction is null)
			_releaseToPoolAction = releaseAction;
	}

	public void JumpImpact()
	{
		Instantiate(_jumpSource, _hammerHitPoint.position, Quaternion.identity);
	}

	public void JumpEnd()
	{
		_canJump = true;
		_animator.SetBool("Jump", false);
		_agent.updatePosition = true;
		_isJumping = false;
	}

	void JumpAttack()
	{
		_canJump = false;
		_animator.SetBool("Jump", true);
		_agent.updatePosition = false;
		_isJumping = true;
	}

	void MeleeAttack()
	{
		_animator.SetInteger("SwingNum", UnityRandom.Range(1, 4));
		_animator.SetBool("IsAttacking", true);
		_animator.SetLayerWeight(_swingLayerIndex, 1.0f);
		_isSwinging = true;
	}

	public void DoDamage()
	{
		_animator.SetBool("IsAttacking", false);
		if (_navPoller.DistanceToPlayer <= _meleeRange)
			_navPoller.TargetPlayer.GetComponent<HealthComponent>().TakeDamage(_damage);
	}

	public void EndSwing()
	{
		_isSwinging = false;
		_agent.updatePosition = true;
		_animator.SetLayerWeight(_swingLayerIndex, 0.0f);
	}

	private void HealthComp_OnDeath(object sender, System.EventArgs e)
	{
		StartCoroutine(nameof(HandleDeath));
	}

	IEnumerator HandleDeath()
	{
		enabled = false;
		_navPoller.enabled = false;
		_agent.enabled = false;
		WaveManager.instance.ReportEnemyKilled();

		_lootDrop.DropLoot();
		_ragdoll.EnableRagdoll();
		_dissolver.StartDissolving();
		yield return new WaitForSeconds(3.0f);
		_releaseToPoolAction(this);
	}

	AudioSource _audioSource;
	Animator _animator;
	NavPollerComponent _navPoller;
	NavMeshAgent _agent;
	HealthComponent _healthComp;
	Dissolver _dissolver;
	RagdollComponent _ragdoll;
	LootDropComponent _lootDrop;
	Action<WarChief> _releaseToPoolAction;

	bool _canJump;
	bool _isJumping;
	bool _isSwinging;
	int _swingLayerIndex;
}
