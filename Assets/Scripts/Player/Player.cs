using System;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Player : MonoBehaviour
{
	public bool isControlled => _playerIndex >= 0;
	public Camera playerCamera;
	[SerializeField] GameObject _avatar;
	
	void Awake()
	{
		_mover = GetComponentInChildren<Mover>();
		_mover.SetPlayer(this);
		_castingComponent = GetComponentInChildren<CastingComponent>();
		_shop = GetComponentInChildren<Shop>();
		_healthComponent = GetComponentInChildren<HealthComponent>();
	}

	void Update()
	{
		UIDebugUtility.instance.UpdateTrackedHealth(_healthComponent.health);
	}

	public Vector3 GetAvatarPosition() => _avatar.transform.position;
	public Vector2 GetAimDirection() => _mover.GetAimDirection();

	public int GetPlayerIndex() => _playerIndex;
    public void SetPlayerIndex(int index) => _playerIndex = index;

	public void MovePlayer(Vector2 input)
	{
		var movement = Vector3.zero;
		movement.x = input.x;
		movement.z = input.y;
		_mover.SetMovement(movement);
	}

	public void SelectAbility(int slotNumber)
	{
		//TODO unify casting animations with abilities and direction
        _castingComponent.SetSelectedAbility(slotNumber);
    }
	
	public void UpdateCasting(bool isCasting)
	{
		_castingComponent.UpdateCasting(isCasting);
	}

	public void SetAiming(bool isAiming, Vector2 aimingDir, bool useMouse)
	{
		_mover.SetAiming(isAiming, aimingDir, useMouse);
	}

	public void ToggleShopUI(bool isEnabled)
	{
		_shop.ToggleShopUI(isEnabled);
	}

	public void OnRoll()
	{
		_mover.OnRoll();
	}

	public Camera PlayerCamera => GetComponentInChildren<Camera>();

	HealthComponent _healthComponent;
	Mover _mover;
	CastingComponent _castingComponent;
	Shop _shop;
	int _playerIndex = -1;
}
