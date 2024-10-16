using UnityEngine;

public class SimplePlayerJoinLogger : MonoBehaviour
{
    void Start()
    {
        // Enable player joining
        PlayerManager.instance.SetJoiningEnabled(true);

        // Subscribe to the PlayerControllerJoined event
        PlayerManager.instance.PlayerControllerJoined += PlayerManager_OnPlayerControllerJoined;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.PlayerControllerJoined -= PlayerManager_OnPlayerControllerJoined;
        }
    }

    void PlayerManager_OnPlayerControllerJoined(object sender, PlayerManager.PlayerJoinedEventArgs e)
    {
        Debug.Log("joined");
    }
}
