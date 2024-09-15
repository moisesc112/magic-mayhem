using UnityEngine;

[RequireComponent(typeof(PlayerStats), typeof(AbilitySlotsComponent))]
public class Player : MonoBehaviour
{
	public bool isControlled => _playerIndex >= 0;
	public Camera playerCamera;
	
	void Awake()
	{
        _abilitySlotsComponent = GetComponent<AbilitySlotsComponent>();
		_mover = GetComponentInChildren<Mover>();
		_mover.SetPlayer(this);
		_castingComponent = GetComponentInChildren<CastingComponent>();
	}

	public void SetPlayerIndex(int index) => _playerIndex = index;

	public void MovePlayer(Vector2 input)
	{
		var movement = Vector3.zero;
		movement.x = input.x;
		movement.z = input.y;
		_mover.SetMovement(movement);
	}

	public void UseAbility(int slotNumber)
	{
		_abilitySlotsComponent.UseAbility(slotNumber);
	}
	
	public void UpdateCasting(bool isCasting)
	{
		_castingComponent.UpdateCasting(isCasting);
	}

	public void SetAiming(bool isAiming, Vector2 aimingDir, bool useMouse)
	{
		_mover.SetAiming(isAiming, aimingDir, useMouse);
	}

	public Camera PlayerCamera => GetComponentInChildren<Camera>();

	Mover _mover;
	AbilitySlotsComponent _abilitySlotsComponent;
	CastingComponent _castingComponent;
	int _playerIndex = -1;
}
