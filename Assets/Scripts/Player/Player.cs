using UnityEngine;

[RequireComponent(typeof(Mover), typeof(PlayerStats), typeof(AbilitySlotsComponent))]
public class Player : MonoBehaviour
{
	public bool isControlled => _playerIndex >= 0;
	public Camera playerCamera;
	
	void Awake()
	{
		_mover = GetComponent<Mover>();
        _abilitySlotsComponent = GetComponent<AbilitySlotsComponent>();
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

	Mover _mover;
	AbilitySlotsComponent _abilitySlotsComponent;
	int _playerIndex = -1;
}
