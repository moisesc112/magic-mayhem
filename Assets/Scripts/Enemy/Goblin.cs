using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
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
		_swingLayerIndex = _animator.GetLayerIndex("Combat");
	}

	void Start()
	{
		RandomizeLook();
		_animator.SetInteger("SwingNum", Random.Range(1, 4));
	}

	void FixedUpdate()
	{
		if (!_healthComp.IsAlive)
			return;

		var distanceToPlayer = _navPoller.DistanceToPlayer;
		if (distanceToPlayer <= _attackRange)
		{
			transform.LookAt(_navPoller.TargetPlayer.GetAvatarPosition());
			if (!_isSwinging)
				StartSwing();
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _attackRange);
	}

	public void StartSwing()
	{
		_animator.SetInteger("SwingNum", Random.Range(1, 4));
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
		var randomSkin = Random.Range(0, _skins.Length);
		var selectedSkin = _skins[randomSkin];
		foreach (var skin in _skins.Where(s => s != selectedSkin))
			skin.SetActive(false);
		GetComponent<RagdollComponent>().SetBoundMesh(selectedSkin.transform);

		selectedSkin.SetActive(true);

		if (Random.Range(0.0f, 1.0f) > _accessoryChance)
			_leftAccessories[Random.Range(0, _leftAccessories.Length)].SetActive(true);
		if (Random.Range(0.0f, 1.0f) > _accessoryChance)
			_rightAccessories[Random.Range(0, _rightAccessories.Length)].SetActive(true);
	}

	AudioSource _audioSource;
	Animator _animator;
	NavPollerComponent _navPoller;
	NavMeshAgent _agent;
	HealthComponent _healthComp;
	bool _isSwinging;
	int _swingLayerIndex;
}
