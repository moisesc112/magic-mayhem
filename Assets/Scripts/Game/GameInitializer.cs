using UnityEngine;

public class GameInitializer : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		var playerControllers = PlayerManager.instance.PlayerControllers;
		foreach (var controller in playerControllers)
		{
			// This could probably be refactored into a dedicated spawner class.
			PlayerManager.instance.SpawnPlayer(controller);
		}
	}
}
