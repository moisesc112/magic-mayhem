using System;

public class GameStateManager : Singleton<GameStateManager>
{
	/// <summary>
	/// Value is bool representing if game was won.
	/// </summary>
	public event EventHandler<GenericEventArgs<bool>> gameEnded;

	// Start is called before the first frame update
	protected override void DoStart()
	{
		if (PlayerManager.instance is null) return; 
		var playerControllers = PlayerManager.instance.PlayerControllers;
		
		foreach (var controller in playerControllers)
		{
			// This could probably be refactored into a dedicated spawner class.
			PlayerManager.instance.SpawnPlayer(controller);
		}

		if (playerControllers.Count > 1)
			PlayerManager.instance.EnableSplitScreen();

		_alivePlayers = playerControllers.Count;

		// Hack for starting from scene other than menu
		if (_alivePlayers == 0)
			PlayerManager.instance.PlayerControllerJoined += PlayerManager_OnPlayerControllerJoined;
	}

	private void PlayerManager_OnPlayerControllerJoined(object sender, GenericEventArgs<PlayerController> e)
	{
		_alivePlayers++;
	}

	private void OnDestroy()
	{
		PlayerManager.instance.PlayerControllerJoined -= PlayerManager_OnPlayerControllerJoined;
	}

	public void HandlePlayerDied(Player player)
	{
		_alivePlayers--;
		if (_alivePlayers == 0)
			RaiseGameLost();
	}

	public void RaiseGameLost()
	{
		gameEnded?.Invoke(this, new GenericEventArgs<bool>(false));
	}

	public void RaiseGameWon()
	{
		gameEnded?.Invoke(this, new GenericEventArgs<bool>(true));
	}

	int _alivePlayers;
}
