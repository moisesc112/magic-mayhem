using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityExtensions;

[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(RagdollComponent))]
[RequireComponent(typeof(LootDropComponent))]
[RequireComponent(typeof(MeleeAttackComponent))]
public class Golem : MonoBehaviour
{
	[Header("FX")]
	[SerializeField] ParticleSystem _dustParticleSystem;
	[SerializeField] AudioClip _stompAudio;
	[SerializeField] Renderer _targetRenderer;

	[Header("Attack")]
	[SerializeField] float _attackRange;
	[SerializeField] RadialDamageSource _stompSource;

	void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();
		_navPoller = GetComponent<NavPollerComponent>();
		_dissolver = GetComponent<Dissolver>();
		if (_targetRenderer)
			_dissolver.SetTargetRenderer(_targetRenderer);
		_ragdoll = GetComponent<RagdollComponent>();
		_lootDrop = GetComponent<LootDropComponent>();
		_healthComp = GetComponent<HealthComponent>();
		_healthComp.onDeath += HealthComp_OnDeath;
		_refreshableComponents = gameObject.GetAllComponents<RefreshableComponent>();
		_meleeAttackComponent = GetComponent<MeleeAttackComponent>();
	}

	private void FixedUpdate()
	{
		if (_meleeAttackComponent.canAttack)
			_meleeAttackComponent.MeleeAttack();
	}

	public void Stomp()
    {
		_dustParticleSystem.Play();
		_audioSource.PlayOneShot(_stompAudio);

		// Particle system is attacked to golem's foot
		var hitPoint = _dustParticleSystem.transform.position;
		Instantiate(_stompSource, hitPoint, Quaternion.identity);
	}

	public void InitFromPool(Vector3 location, Action<Golem> releaseAction)
	{
		transform.position = location;
		_agent.transform.position = transform.position;

		enabled = true;
		_navPoller.enabled = true;
		_agent.enabled = true;

		_dissolver.ResetEffect();

		_healthComp.health = _healthComp.maxHealth;
		_ragdoll.DisableRagdoll();

		_navPoller.StartPolling();

		if (_refreshableComponents != null)
		{
			foreach (var comp in _refreshableComponents)
			{
				comp.OnInit();
			}
		}

		if (_releaseToPoolAction is null)
			_releaseToPoolAction = releaseAction;
	}

	void HealthComp_OnDeath(object sender, EventArgs e)
	{
		StartCoroutine(nameof(HandleDeath));
	}

	IEnumerator HandleDeath()
	{
		enabled = false;
		_navPoller.enabled = false;
		_agent.enabled = false;
		WaveManager.instance?.ReportEnemyKilled();

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
		_releaseToPoolAction(this);
	}

	AudioSource _audioSource;
	Animator _animator;
	NavPollerComponent _navPoller;
	NavMeshAgent _agent;
	Dissolver _dissolver;
	HealthComponent _healthComp;
	RagdollComponent _ragdoll;
	LootDropComponent _lootDrop;
	Action<Golem> _releaseToPoolAction;
	RefreshableComponent[] _refreshableComponents;
	MeleeAttackComponent _meleeAttackComponent;
}
