using UnityEngine;

public class Player : MonoBehaviour
{
	public bool isControlled => _playerIndex >= 0;
	public Camera playerCamera;
	
	void Awake()
	{
		_mover = GetComponentInChildren<Mover>();
		_mover.SetPlayer(this);
		_castingComponent = GetComponentInChildren<CastingComponent>();
	}

	public Vector3 GetAvatarPosition() => transform.position;

	public void SetPlayerIndex(int index) => _playerIndex = index;

	public void MovePlayer(Vector2 input)
	{
		var movement = Vector3.zero;
		movement.x = input.x;
		movement.z = input.y;
		_mover.SetMovement(movement);
	}

	public void UpdateCasting(bool isCasting)
	{
		_castingComponent.UpdateCasting(isCasting);
	}

	public void SetAiming(bool aiming)
	{
		_mover.SetAiming(aiming);
	}

	public Camera PlayerCamera => GetComponentInChildren<Camera>();

	Mover _mover;
	CastingComponent _castingComponent;
	int _playerIndex = -1;
}
