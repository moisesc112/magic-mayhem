using UnityEngine;

[RequireComponent (typeof(Mover))]
public class Player : MonoBehaviour
{
	public bool isControlled => _playerIndex >= 0;
	
	void Awake()
	{
		_mover = GetComponent<Mover>();
	}

	public void SetPlayerIndex(int index) => _playerIndex = index;

	public void MovePlayer(Vector2 input)
	{
		var movement = Vector3.zero;
		movement.x = input.x;
		movement.z = input.y;
		_mover.SetMovement(movement);
	}

	Mover _mover;
	int _playerIndex = -1;
}
