using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class Mover : MonoBehaviour
{
	[Header("Speed Settings")]
	[SerializeField] float _maxPlayerSpeed = 4.8f;
	[SerializeField] float _rotationSpeedDegrees = 180.0f;
	[SerializeField] float _animSpeed = 1.0f;
	[SerializeField] float _rootOverride = 1.0f;
	[SerializeField] float _acceleration = 0.1f;

	public TextMeshProUGUI playerVelocityCounter;

	void Awake()
	{
		_anim = GetComponent<Animator>();
		_characterController = GetComponent<CharacterController>();
		_anim.applyRootMotion = true;
	}

	private void Start()
	{
		playerVelocityCounter = GameObject.FindGameObjectWithTag("VelocityTracker").GetComponent<TextMeshProUGUI>();
	}

	void Update()
	{
		_currentMovement = _targetMovement;
		UpdateGravity();
		UpdateAnimParams();
		UpdateRotation();
	}

	void OnAnimatorMove()
	{
		Animator anim = GetComponentInChildren<Animator>();
		if (!anim)
			return;

		var targetMovement = _anim.deltaPosition * _rootOverride;
		targetMovement.y = _gravitySpeed * Time.deltaTime;

		_characterController.Move(targetMovement);
	}

	public void SetMovement(Vector3 movement) => _targetMovement = movement;
	
	public void SetAiming(bool aiming, Vector2 aimDir, bool useMouse)
	{
		if (_isAiming != aiming && !aiming) // If we transtion from aiming to not aiming, rotate player to match input
		{
			var targetRot = Quaternion.LookRotation(_currentMovement, Vector3.up);
			var angleBetween = Vector3.Angle(transform.forward, _currentMovement);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, angleBetween * 0.5f);
		}
		_anim.SetBool("IsAiming", aiming);
		_isAiming = aiming;
		_useMouse = useMouse;

		_aimDirection = aimDir;
		if (aimDir != Vector2.zero)
			_prevAimDirection = aimDir;
    }

	public void SetPlayer(Player player) => _player = player;

	void UpdateGravity()
	{
        if (_characterController.isGrounded)
			_gravitySpeed = 0;
		else
			_gravitySpeed += Physics.gravity.y * Time.deltaTime;
	}

	void UpdateAnimParams()
	{
		var inputAmount = _currentMovement.magnitude;
		var targetForward = 0.0f;
		var targetRight = 0.0f;

		_isMoving = Mathf.Abs(inputAmount) > 0.1;
		if (_useMouse)
			_aimDirection = GetMouseDirection();

		if (_isAiming) // Player maintains rotation while moving
		{
			if (_isMoving)
			{
				var localInput = transform.InverseTransformDirection(_currentMovement);
				targetForward = localInput.z;
				targetRight =  localInput.x;
			}
			else // Player rotates to aim position (mouse)
			{
				var dirToMouse3D = new Vector3(_aimDirection.x, 0, _aimDirection.y);
				var turnDirection = Vector3.Cross(transform.forward, dirToMouse3D).y;
				
				// Cross product can produce a vector of mag 0 if vectors are parallel.
				// If this happens, fall back to dot product to see if they are aligned or opposed.
				if (turnDirection == float.NaN)
					turnDirection = Vector3.Dot(transform.forward, dirToMouse3D) > 0 ? 0.0f : 1.0f;

				targetRight = turnDirection;
			}
		}
		else // Player rotates to face movement direction
		{
			targetForward = _maxPlayerSpeed * inputAmount;
		}

		var forward = Mathf.MoveTowards(_anim.GetFloat("Forward"), targetForward, _acceleration);
		_anim.SetFloat("Forward", forward);

		var right = Mathf.MoveTowards(_anim.GetFloat("Right"), targetRight, _acceleration);
		_anim.SetFloat("Right", right);

		_anim.SetBool("IsMoving", _isMoving);

		_anim.speed = _animSpeed;
	}

	void UpdateRotation()
	{
		if (_isAiming)
		{
			var aimDir = _useMouse ? GetMouseDirection() : _aimDirection;
			var targetDir = new Vector3(aimDir.x, 0, aimDir.y);
			var lookRotation = Quaternion.LookRotation(targetDir, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotationSpeedDegrees * Time.deltaTime);
		}
		else // Rotate to face direction of movement
		{
			var lookRotation = _currentMovement == Vector3.zero 
				? transform.rotation 
				: Quaternion.LookRotation(_currentMovement, Vector3.up);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotationSpeedDegrees * Time.deltaTime);
		}
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

	public Vector2 GetAimDirection() => _useMouse ? GetMouseDirection() : _prevAimDirection.normalized;

	Animator _anim;
	CharacterController _characterController;
	Player _player;

	Vector3 _currentMovement;
	Vector3 _targetMovement;
	Vector2 _aimDirection;
	Vector2 _prevAimDirection;
	bool _isAiming;
	bool _isMoving;
	bool _useMouse;
	float _gravitySpeed;
}
