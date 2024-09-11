using UnityEngine;

public class GameInitializer : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
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
	}
}
