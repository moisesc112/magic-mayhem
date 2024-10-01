using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class RagdollComponent : MonoBehaviour
{

	[Header("Optional")]
	[SerializeField] NavMeshAgent _agent;
	[SerializeField] bool _trackRoot;
	[SerializeField] Transform _boundMesh;
	[SerializeField] Transform _centerOfMass;
	[SerializeField] bool _disableOwnCollider = true;

	void Awake()
	{
		_ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
		_ragdollColliders = GetComponentsInChildren<Collider>();
		_animator = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();

		_ownCollider = GetComponent<Collider>();
		_ownRigidBody = GetComponent<Rigidbody>();
		DisableRagdoll();
		_isRagdolling = false;
	}

	void Update()
	{
		if (_trackRoot && _isRagdolling)
		{
			_boundMesh.transform.position = _centerOfMass.position;
		}
	}

	public void SetBoundMesh(Transform boundMesh)
	{
		_boundMesh = boundMesh;
	}

	public void EnableRagdoll()
	{
		_isRagdolling = true;
		foreach (var rb in _ragdollRigidBodies)
			rb.isKinematic = false;

		foreach (var collider in _ragdollColliders)
			collider.enabled = true;

		// This Rigidbody should always be kinematic. Ensure it stays that way after interating through children.
		_ownRigidBody.isKinematic = true;

		if (_agent)
			_agent.enabled = false;
		_animator.enabled = false;

		if (_disableOwnCollider)
			_ownCollider.enabled = false;
	}

	public void DisableRagdoll()
	{
		_isRagdolling = false;
		foreach (var rb in _ragdollRigidBodies)
			rb.isKinematic = true;

		foreach (var collider in _ragdollColliders)
			collider.enabled = false;

		_ownCollider.enabled = true;

		if (_agent)
			_agent.enabled = true;
		_animator.enabled = true;
	}

	bool _isRagdolling;
	Rigidbody[] _ragdollRigidBodies;
	Collider[] _ragdollColliders;
	Animator _animator;
	Collider _ownCollider;
	Rigidbody _ownRigidBody;
}
