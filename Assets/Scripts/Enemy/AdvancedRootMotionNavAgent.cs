using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AdvancedRootMotionNavAgent : MonoBehaviour
{
	Animator anim;
	NavMeshAgent agent;
	[SerializeField] float _rootOverride = 1.0f;
	Vector2 smoothDeltaPosition = Vector2.zero;
	Vector2 velocity = Vector2.zero;

	void Awake()
	{
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

		anim.applyRootMotion = true;
		agent.updatePosition = false;
	}

	void Update()
	{
		Vector3 worldDeltaPosition = (agent.nextPosition - transform.position);
		worldDeltaPosition.y = 0;

		// Map 'worldDeltaPosition' to local space
		float dx = Vector3.Dot(transform.right, worldDeltaPosition);
		float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
		Vector2 deltaPosition = new Vector2(dx, dy);

		// Low-pass filter the deltaMove
		float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.2f);
		smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

		velocity = smoothDeltaPosition / Time.deltaTime;
		if (agent.remainingDistance <= agent.stoppingDistance)
			velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance / agent.stoppingDistance);

		bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

		// Update animation parameters
		anim.SetBool("IsMoving", shouldMove);
		anim.SetFloat("VelX", velocity.x);
		anim.SetFloat("VelY", velocity.y);

		if (worldDeltaPosition.magnitude > agent.radius)
			transform.position = agent.nextPosition - 0.9f * worldDeltaPosition;
	}


	// TODO: Use root override
	void OnAnimatorMove()
	{
		Vector3 position = anim.rootPosition;
		position.y = agent.nextPosition.y;
		transform.position = position;
		agent.nextPosition = position;
	}
}