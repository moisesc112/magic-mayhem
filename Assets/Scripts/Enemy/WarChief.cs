using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityExtensions;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(RagdollComponent))]
[RequireComponent(typeof(LootDropComponent))]
[RequireComponent(typeof(MeleeAttackComponent))]
public class WarChief : MonoBehaviour
{
	[Header("Attack")]
	[SerializeField] float _jumpRangeStartSq = 36.0f;
	[SerializeField] RadialDamageSource _jumpSource;
	[SerializeField] Transform _hammerHitPoint;
	[SerializeField] float _jumpCooldown = 15.0f;

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
		_rootMotionNavAgent = GetComponent<SimpleRootMotionNavAgent>();
		_meleeAttackComponent = GetComponent<MeleeAttackComponent>();
		_refreshableComponents = gameObject.GetAllComponents<RefreshableComponent>();
	}

	private void FixedUpdate()
	{
		if (_navPoller.DistanceToPlayer >= _jumpRangeStartSq)
			_canJump = true;
		else if (_canJump && !_isJumping && _navPoller.DistanceToPlayer < _jumpRangeStartSq)
			JumpAttack();
		else if (!_isJumping && _meleeAttackComponent.canAttack)
			_meleeAttackComponent.MeleeAttack();
	}

	private void OnDestroy()
	{
		_healthComp.onDeath -= HealthComp_OnDeath;
	}

	public void InitFromPool(Vector3 location, Action<WarChief> releaseAction)
	{
		transform.position = location;
		_agent.transform.position = transform.position;

		enabled = true;
		_navPoller.enabled = true;
		_agent.enabled = true;
		_rootMotionNavAgent.enabled = true;

		_canJump = true;

		_dissolver.ResetEffect();

		_healthComp.health = _healthComp.maxHealth;
		_ragdoll.DisableRagdoll();

		_navPoller.StartPolling();

		if (_refreshableComponents != null)
		{
			foreach (var component in _refreshableComponents)
			{
				component.OnInit();
			}
		}

		if (_releaseToPoolAction is null)
			_releaseToPoolAction = releaseAction;
	}

	public void JumpImpact()
	{
		Instantiate(_jumpSource, _hammerHitPoint.position, Quaternion.identity);
	}

	public void JumpEnd()
	{
		_isJumping = false;
	}

	void JumpAttack()
	{
		_animator.SetTrigger("Jump");
		_animator.SetTrigger("Attack");
		_canJump = false;
		_isJumping = true;
		StartCoroutine(nameof(JumpCooldown));
	}

	public void DoDamage()
	{
		_animator.SetBool("IsAttacking", false);
	}

	public void EndSwing()
	{
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
		_canJump = true;
		_isJumping = false;
		_rootMotionNavAgent.enabled = false;
		WaveManager.instance.ReportEnemyKilled();

		_lootDrop.DropLoot();
		_ragdoll.EnableRagdoll();
		_dissolver.StartDissolving();
		if (_refreshableComponents != null)
		{
			foreach (var comp in _refreshableComponents)
			{
				comp.OnKilled();
			}
		}
		yield return new WaitForSeconds(3.0f);
		_releaseToPoolAction?.Invoke(this);
	}

	IEnumerator JumpCooldown()
	{
		yield return new WaitForSeconds(_jumpCooldown);
		_canJump = true;
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
	SimpleRootMotionNavAgent _rootMotionNavAgent;
	MeleeAttackComponent _meleeAttackComponent;
	RefreshableComponent[] _refreshableComponents;

	bool _canJump;
	bool _isJumping;
	bool _canSwing = true;
	int _swingLayerIndex;
}
