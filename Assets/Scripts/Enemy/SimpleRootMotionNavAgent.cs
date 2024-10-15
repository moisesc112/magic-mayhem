using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(NavMeshAgent))]
public class SimpleRootMotionNavAgent : MonoBehaviour
{
	[SerializeField] float _rotationSpeedDegrees = 180.0f;
	[SerializeField] float _velocitySmoothing = 0.2f;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();

		_animator.applyRootMotion = true;
		_agent.updatePosition = false;
		_agent.updateRotation = true;
	}

	void Update()
	{
		var velocity = _agent.velocity.magnitude;
		_animator.SetBool("IsMoving", velocity > 0.1f && _agent.remainingDistance > _agent.radius);
		_animator.SetFloat("Speed", velocity);
	}

	void OnAnimatorMove()
	{
		var position = _animator.rootPosition;
		position.y = _agent.nextPosition.y;
		transform.position = position;
		_agent.nextPosition = position;
	}

	Animator _animator;
	NavMeshAgent _agent;
}
