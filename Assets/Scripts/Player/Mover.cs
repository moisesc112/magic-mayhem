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
		Animator anim = GetComponentInChildren<Animator>();
		if (!anim)
			return;

		Vector3 newRootPos = Vector3.LerpUnclamped(transform.position, _anim.rootPosition, _rootMovementSpeed);
		Quaternion newRootRot;
		
		
		if (_isMoving) // Drive rotation manualy, but still use root motion position
		{

			Quaternion targetRotation = _isAiming
				? Quaternion.LookRotation(new Vector3(_aimDirection.x, 0, _aimDirection.y), Vector3.up)
				: Quaternion.LookRotation(_currentMovement, Vector3.up);
			newRootRot = Quaternion.LerpUnclamped(transform.rotation, targetRotation, _rootRotationSpeed * Time.deltaTime);
		}
		else // Drive rotation via root motion since motion wont hide any foot sliding
		{
			newRootRot = Quaternion.LerpUnclamped(transform.rotation, _anim.rootRotation, _rootRotationSpeed);
		}

		_rigidbody.MovePosition(newRootPos);
		_rigidbody.MoveRotation(newRootRot);
	}

	public void SetMovement(Vector3 movement) => _targetMovement = movement;
	
	public void SetAiming(bool aiming, Vector2 aimDir, bool useMouse)
	{
		_anim.SetBool("IsAiming", aiming);
		_isAiming = aiming;
		_useMouse = useMouse;

		Debug.Log(aimDir);
		_aimDirection = aimDir;
	}

	public void SetPlayer(Player player) => _player = player;

	void HandleMovement()
	{
		var inputAmount = _currentMovement.magnitude;
		_isMoving = Mathf.Abs(inputAmount) > 0.1;
		if (_useMouse)
			_aimDirection = GetMouseDirection();

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
				var dirToMouse3D = new Vector3(_aimDirection.x, 0, _aimDirection.y);
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
			_anim.SetFloat("Forward", inputAmount);
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
	Vector2 _aimDirection;
	bool _isAiming;
	bool _isMoving;
	bool _useMouse;
}
