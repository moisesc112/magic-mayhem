using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class LobbyManager : MonoBehaviour
{
    private PlayerController lobbyLeaderController;
    private InputSystemUIInputModule lobbyLeaderInputModule;

    void Start()
    {
        // Enable player joining
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.SetJoiningEnabled(true);

            // Subscribe to the PlayerControllerJoined event
            PlayerManager.instance.PlayerControllerJoined += OnPlayerControllerJoined;
        }
        else
        {
            Debug.LogError("PlayerManager instance not found. Ensure that PlayerManager is present in the scene.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from the PlayerControllerJoined event
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.PlayerControllerJoined -= OnPlayerControllerJoined;
        }

        // Unsubscribe from input actions
        if (lobbyLeaderInputModule != null)
        {
            lobbyLeaderInputModule.submit.action.performed -= OnSubmit;
            lobbyLeaderInputModule.cancel.action.performed -= OnCancel;
            lobbyLeaderInputModule.move.action.performed -= OnMove;
        }
    }

    void OnPlayerControllerJoined(object sender, PlayerManager.PlayerJoinedEventArgs e)
    {
        if (lobbyLeaderController == null)
        {
            // Assign the first player as the lobby leader
            lobbyLeaderController = e.playerController;

            Debug.Log($"Lobby Leader Assigned: Player {lobbyLeaderController.playerIndex}");

            // Disable further player joining until the next screen
            PlayerManager.instance.SetJoiningEnabled(false);

            // Set up input handling for the lobby leader
            SetupLobbyLeaderInput();
        }
        else
        {
            // Ignore inputs from other players for now
            Debug.Log("Another player tried to join, but we're only accepting inputs from the lobby leader at this time.");
        }
    }

    void SetupLobbyLeaderInput()
    {
        // Access the EventSystem in the scene
        EventSystem eventSystem = EventSystem.current;

        if (eventSystem != null)
        {
            // Get the InputSystemUIInputModule component
            lobbyLeaderInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();

            if (lobbyLeaderInputModule != null)
            {
                // Assign the lobby leader's actions to the InputSystemUIInputModule
                lobbyLeaderInputModule.actionsAsset = lobbyLeaderController.playerInput.actions;

                // Assign the InputSystemUIInputModule to the lobby leader's PlayerInput
                lobbyLeaderController.playerInput.uiInputModule = lobbyLeaderInputModule;

                // Subscribe to actions
                lobbyLeaderInputModule.submit.action.performed += OnSubmit;
                lobbyLeaderInputModule.cancel.action.performed += OnCancel;
                lobbyLeaderInputModule.move.action.performed += OnMove;
            }
            else
            {
                Debug.LogError("InputSystemUIInputModule not found on the EventSystem.");
            }
        }
        else
        {
            Debug.LogError("EventSystem not found in the scene.");
        }
    }

    void OnSubmit(InputAction.CallbackContext context)
    {
        // Handle the submit action
        Debug.Log("Lobby Leader pressed Submit.");

        // Implement your logic here (e.g., proceed to the next screen)
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        // Handle the cancel action
        Debug.Log("Lobby Leader pressed Cancel.");

        // Implement your logic here (e.g., go back or exit)
    }

    void OnMove(InputAction.CallbackContext context)
    {
        // Handle the move action
        Vector2 movement = context.ReadValue<Vector2>();
        Debug.Log($"Lobby Leader moved: {movement}");

        // Implement your logic here (e.g., navigate UI)
    }

    // Optional: Method to access the lobby leader's controller from other scripts
    public PlayerController GetLobbyLeaderController()
    {
        return lobbyLeaderController;
    }
}
