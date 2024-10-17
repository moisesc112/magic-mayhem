using UnityEngine;

public class MenuButtonHandler : MonoBehaviour
{
    [Header("Lobby Manager")]
    public LobbyManager lobbyManager; // Assign via Inspector

    public void OnSettingsButtonPressed()
    {
        Debug.Log("Settings Button Pressed.");
        // Implement Settings functionality here
    }

    public void OnCreditsButtonPressed()
    {
        Debug.Log("Credits Button Pressed.");
        // Implement Credits functionality here
    }

    public void OnLobbyButtonPressed()
    {
        Debug.Log("Lobby Button Pressed.");
        if (lobbyManager != null)
        {
            lobbyManager.MoveCameraToLobby();
        }
        else
        {
            Debug.LogWarning("LobbyManager is not assigned in MenuButtonHandler.");
        }
    }
}
