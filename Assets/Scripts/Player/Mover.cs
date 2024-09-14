using System;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
	[Header("Speed Settings")]
	[SerializeField] float _movementAcceleration = 5.0f;
	[SerializeField] float _animSpeed = 1.0f;
	[SerializeField] float _rootMovementSpeed = 1.0f;
	[SerializeField] float _rootRotationSpeed = 1.0f;

	void Awake()
	{
		_anim = GetComponent<Animator>();
		_rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		_currentMovement = Vector3.Lerp(_currentMovement, _targetMovement, Time.deltaTime * _movementAcceleration);
		HandleMovement();
	}

	void OnAnimatorMove()
	{
		// Sometimes anim speed is CRAZY high. Why is that?
		Animator anim = GetComponentInChildren<Animator>();
		if (!anim)
			return;

		Vector3 newRootPos = Vector3.LerpUnclamped(transform.position, _anim.rootPosition, _rootMovementSpeed);
		Quaternion newRootRot;
		
		if (_isAiming && _isMoving)
		{
			var targetRotation = Quaternion.LookRotation(new Vector3(_directionToMouse.x, 0, _directionToMouse.y), Vector3.up);
			newRootRot = Quaternion.LerpUnclamped(transform.rotation, targetRotation, _rootRotationSpeed * Time.deltaTime);
		}
		else
		{
			newRootRot = Quaternion.LerpUnclamped(transform.rotation, _anim.rootRotation, _rootRotationSpeed);
		}

		_rigidbody.MovePosition(newRootPos);
		_rigidbody.MoveRotation(newRootRot);
	}

	public void SetMovement(Vector3 movement) => _targetMovement = movement;
	
	public void SetAiming(bool aiming)
	{
		_anim.SetBool("IsAiming", aiming);
		_isAiming = aiming;
	}

	public void SetPlayer(Player player) => _player = player;

	void HandleMovement()
	{
		var inputDir = _currentMovement.normalized;
		var inputAmount = _currentMovement.magnitude;
		_isMoving = Mathf.Abs(inputAmount) > 0.1;
		_directionToMouse = GetMouseDirection();

		if (_isAiming) // Player maintains rotation while moving
		{
			if (_isMoving)
			{
				var localInput = transform.InverseTransformDirection(_currentMovement);
				_anim.SetFloat("Forward", localInput.z);
				_anim.SetFloat("Right", localInput.x);
			}
			else // Player rotates to aim position (mouse)
			{
				var dirToMouse3D = new Vector3(_directionToMouse.x, 0, _directionToMouse.y);
				var turnDirection = Vector3.Cross(transform.forward, dirToMouse3D).y;
				
				// Cross product can produce a vector of mag 0 if vectors are parallel.
				// If this happens, fall back to dot product to see if they are aligned or opposed.
				if (turnDirection == float.NaN)
					turnDirection = Vector3.Dot(transform.forward, dirToMouse3D) > 0 ? 0.0f : 1.0f;
				
				_anim.SetFloat("Right", turnDirection);
			}
		}
		else // Player rotates to face movement direction
		{
			var facing = transform.forward;

			var alignment = Vector3.Dot(facing, inputDir);
			float turnAmount;
			if (alignment == 1)
			{
				turnAmount = 0;
			}
			else
			{
				var turnDirection = Vector3.Cross(facing, _currentMovement);
				turnAmount = Mathf.Sign(turnDirection.y) * Mathf.Acos(alignment);
			}
			
			_anim.SetFloat("Forward", inputAmount);
			_anim.SetFloat("Right", turnAmount * inputAmount);
		}

		_anim.SetBool("IsMoving", _isMoving);

		_anim.speed = _animSpeed;
	}

	/// <summary>
	/// Returns normalized Vector2 pointing from Player to mouse cursor.
	/// </summary>
	Vector2 GetMouseDirection()
	{
		var cursorPos = Input.mousePosition;
		var playerPos = _player.PlayerCamera.WorldToScreenPoint(gameObject.transform.position);
		return (cursorPos - playerPos).normalized;
	}

	Animator _anim;
	Rigidbody _rigidbody;
	Player _player;

	Vector3 _currentMovement;
	Vector3 _targetMovement;
	Vector2 _directionToMouse;
	bool _isAiming;
	bool _isMoving;
}
