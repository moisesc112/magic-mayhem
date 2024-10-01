using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(HealthComponent))]
public class WarChief : MonoBehaviour
{
	[Header("Attack")]
	[SerializeField] float _meleeRange;
	[SerializeField] float _jumpRangeStart = 6.0f;
	[SerializeField] float _jumpRangeEnd = 12.0f;
	[SerializeField] float _damage;
	[SerializeField] RadialDamageSource _jumpSource;
	[SerializeField] Transform _hammerHitPoint;

	void Start()
    {
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();
		_navPoller = GetComponent<NavPollerComponent>();
		_agent = GetComponent<NavMeshAgent>();
		_healthComp = GetComponent<HealthComponent>();

		_swingLayerIndex = _animator.GetLayerIndex("Combat");
		_canJump = true;
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
			transform.LookAt(_navPoller.TargetPlayer.GetAvatarPosition());
			if (!_isSwinging)
				MeleeAttack();
		}
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
		_animator.SetInteger("SwingNum", Random.Range(1, 4));
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

	AudioSource _audioSource;
	Animator _animator;
	NavPollerComponent _navPoller;
	NavMeshAgent _agent;
	HealthComponent _healthComp;

	bool _canJump;
	bool _isJumping;
	bool _isSwinging;
	int _swingLayerIndex;
}
