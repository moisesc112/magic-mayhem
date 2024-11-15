using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerManager : Singleton<PlayerManager>
{
	[Header("Settings")]
	public Player playerPrefab;
	public bool spawnPlayerOnConnect = true;
	public Vector3 spawnLocation;

	public ReadOnlyCollection<PlayerController> PlayerControllers => _playersByOwningController.Keys.ToList().AsReadOnly();

	public event EventHandler<GenericEventArgs<PlayerController>> PlayerControllerJoined;
	public event EventHandler<GenericEventArgs<int>> PlayerControllerRemoved;

	protected override void DoAwake()
	{
		_playersByOwningController = new Dictionary<PlayerController, Player>();
		_inputManager = GetComponent<PlayerInputManager>();

		DontDestroyOnLoad(gameObject);
	}

	public void RegisterPlayer(PlayerController controller)
	{
		_playersByOwningController.Add(controller, null);
		if (spawnPlayerOnConnect)
			SpawnPlayer(controller);

		PlayerControllerJoined?.Invoke(sender:this, new GenericEventArgs<PlayerController>(controller));
	}

	public void RemovePlayer(int index)
	{
		var matchingController = _playersByOwningController.Keys.FirstOrDefault(c => c.playerIndex == index);
		if (matchingController is null) return;

		matchingController.ReleaseControl();
		_playersByOwningController.Remove(matchingController);

		Destroy(matchingController.gameObject);

		PlayerControllerRemoved?.Invoke(sender: this, new GenericEventArgs<int>(index));
	}

	public void EnableSplitScreen()
	{
		foreach (var controller in _playersByOwningController.Keys)
			controller.SetUpInputCamera();
		_inputManager.splitScreen = true;
	}

	public Player SpawnPlayer(PlayerController owningController)
	{
		EnsurePlayerIsRegistered(owningController);

		// Update this to spawn players at designated places around the map.
		// Use tag PlayerControllerClone to find the instantiations of player controller
        // when we need to restart game
		var player = Instantiate(playerPrefab, spawnLocation, Quaternion.identity);
		owningController.TakeControl(player);
		owningController.gameObject.tag = "PlayerControllerClone";
		_playersByOwningController[owningController] = player;
		return player;
	}

	public void OnPlayerLeft(PlayerInput input)
	{
		var controller = PlayerControllers.FirstOrDefault(c => c.playerInput == input);
		if (controller)
			OnPlayerLeft(controller);
	}

	public void OnPlayerLeft(PlayerController controller)
	{
		// If player isn't registered, nothing to do.
		if (!_playersByOwningController.TryGetValue(controller, out var existingPlayer))
			return;

		controller.ReleaseControl();
	}

	public void OnPlayerRejoined(PlayerController controller)
	{
		EnsurePlayerIsRegistered(controller);
		var existingPlayer = _playersByOwningController[controller];

		controller.TakeControl(existingPlayer);
	}

	public void SetJoiningEnabled(bool isEnabled)
	{
		if (isEnabled)
			_inputManager.EnableJoining();
		else
			_inputManager.DisableJoining();
	}

	void EnsurePlayerIsRegistered(PlayerController controller)
	{
		if (!_playersByOwningController.TryGetValue(controller, out var existingPlayer))
			RegisterPlayer(controller);
	}

	Dictionary<PlayerController, Player> _playersByOwningController;
	PlayerInputManager _inputManager;
}
