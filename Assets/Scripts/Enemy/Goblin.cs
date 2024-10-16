using System;
using System.Collections;
using System.Linq;
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
public class Goblin : MonoBehaviour
{
	[Header("Appearance")]
	[SerializeField] GameObject[] _leftAccessories;
	[SerializeField] GameObject[] _rightAccessories;
	[SerializeField] GameObject[] _skins;
	[SerializeField] float _accessoryChance = 0.25f;

	[Header("Attack")]
	[SerializeField] float _attackRange;
	[SerializeField] float _damage;

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

		var distanceToPlayer = _navPoller.DistanceToPlayer;
		if (distanceToPlayer <= _attackRange)
		{
			transform.LookAt(_navPoller.TargetPlayer?.GetAvatarPosition() ?? Vector3.zero);
			if (!_isSwinging)
				StartSwing();
		}
	}

	private void OnDestroy()
	{
		_healthComp.onDeath -= HealthComp_OnDeath;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _attackRange);
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

		if (_releaseToPoolAction is null)
			_releaseToPoolAction = releaseAction;
	}

	public void StartSwing()
	{
		_animator.SetInteger("SwingNum", UnityRandom.Range(1, 4));
		_animator.SetBool("IsAttacking", true);
		_animator.SetLayerWeight(_swingLayerIndex, 1.0f);
		_isSwinging = true;
		//_agent.updatePosition = false;
	}

	public void DoDamage()
	{
		_animator.SetBool("IsAttacking", false);
		if (_navPoller.DistanceToPlayer <= _attackRange)
			_navPoller.TargetPlayer.GetComponent<HealthComponent>().TakeDamage(_damage);
	}

	public void EndSwing()
	{
		_isSwinging = false;
		_agent.updatePosition = true;
		_animator.SetLayerWeight(_swingLayerIndex, 0.0f);
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
	Action<Goblin> _releaseToPoolAction;

	bool _isSwinging;
	int _swingLayerIndex;
}
