using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityExtensions;
using UnityRandom = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(RagdollComponent))]
[RequireComponent(typeof(LootDropComponent))]
public class Goblin : MonoBehaviour
{
	[Header("Appearance")]
	[SerializeField] GameObject[] _leftAccessories;
	[SerializeField] GameObject[] _rightAccessories;
	[SerializeField] GameObject[] _skins;
	[SerializeField] float _accessoryChance = 0.25f;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();
		_navPoller = GetComponent<NavPollerComponent>();
		_agent = GetComponent<NavMeshAgent>();
		_healthComp = GetComponent<HealthComponent>();

		_healthComp.onDeath += HealthComp_OnDeath;
		_swingLayerIndex = _animator.GetLayerIndex("Combat");
		_dissolver = GetComponent<Dissolver>();
		_ragdoll = GetComponent<RagdollComponent>();
		_lootDrop = GetComponent<LootDropComponent>();
		_refreshableComponents = gameObject.GetAllComponents<RefreshableComponent>();
		_meleeAttackComponent = GetComponent<MeleeAttackComponent>();
	}

	void Start()
	{
		RandomizeLook();
		_animator.SetInteger("SwingNum", UnityRandom.Range(1, 4));
	}

	void FixedUpdate()
	{
		if (!_healthComp.IsAlive)
			return;

		if (_meleeAttackComponent.canAttack)
			_meleeAttackComponent.MeleeAttack();
	}

	private void OnDestroy()
	{
		_healthComp.onDeath -= HealthComp_OnDeath;
	}

	/// <summary>
	/// Called when spawned via pool
	/// </summary>
	public void InitFromPool(Vector3 location, Action<Goblin> releaseAction)
	{
		transform.position = location;
		_agent.transform.position = transform.position;

		enabled = true;
		_navPoller.enabled = true;
		_agent.enabled = true;

		_dissolver.ResetEffect();
		RandomizeLook();

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

	void RandomizeLook()
	{
		var randomSkin = UnityRandom.Range(0, _skins.Length);
		var selectedSkin = _skins[randomSkin];
		foreach (var skin in _skins.Where(s => s != selectedSkin))
			skin.SetActive(false);

		_dissolver.SetTargetRenderer(selectedSkin.GetComponent<Renderer>());

		GetComponent<RagdollComponent>().SetBoundMesh(selectedSkin.transform);

		selectedSkin.SetActive(true);

		if (UnityRandom.Range(0.0f, 1.0f) > _accessoryChance)
			_leftAccessories[UnityRandom.Range(0, _leftAccessories.Length)].SetActive(true);
		if (UnityRandom.Range(0.0f, 1.0f) > _accessoryChance)
			_rightAccessories[UnityRandom.Range(0, _rightAccessories.Length)].SetActive(true);
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
		if (WaveManager.instance != null)
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

	AudioSource _audioSource;
	Animator _animator;
	NavPollerComponent _navPoller;
	NavMeshAgent _agent;
	HealthComponent _healthComp;
	Dissolver _dissolver;
	RagdollComponent _ragdoll;
	LootDropComponent _lootDrop;
	Action<Goblin> _releaseToPoolAction;
	RefreshableComponent[] _refreshableComponents;
	MeleeAttackComponent _meleeAttackComponent;

	bool _isSwinging;
	int _swingLayerIndex;
}
