using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(CharacterController))]
public class RagdollComponent : MonoBehaviour
{
	void Awake()
	{
		_ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
		_selfRigidBody = GetComponent<Rigidbody>();
		_animator = GetComponent<Animator>();
		_characterController = GetComponent<CharacterController>();
		SetRagdollState(shouldRagdoll: false);
	}

	void Update()
	{
		if (_shouldRagdoll)
		{
			transform.position = _position;
			transform.rotation = _rotation;
		}
	}

	public void SetRagdollState(bool shouldRagdoll)
	{
		_shouldRagdoll = shouldRagdoll;
		if (shouldRagdoll)
		{
			_position = transform.position;
			_rotation = transform.rotation;

			if (_characterController.isGrounded)
				transform.position = transform.position + (Vector3.up * 1.4f);
		}

		foreach(var rb in _ragdollRigidBodies)
		{
			rb.isKinematic = !shouldRagdoll;
			_animator.enabled = !shouldRagdoll;
			_characterController.enabled = !shouldRagdoll;
			//_selfRigidBody.isKinematic = shouldRagdoll;
		}
	}

	Rigidbody[] _ragdollRigidBodies;
	Rigidbody _selfRigidBody;
	Animator _animator;
	CharacterController _characterController;
	Vector3 _position;
	Quaternion _rotation;
	bool _shouldRagdoll;
}
