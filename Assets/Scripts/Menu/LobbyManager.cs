using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class LobbyManager : MonoBehaviour
{
    private PlayerController lobbyLeaderController;
    private InputSystemUIInputModule lobbyLeaderInputModule;

    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject gameCanvas;

    void Start()
    {
        // Enable player joining
        PlayerManager.instance.SetJoiningEnabled(true);

        // Subscribe to the PlayerControllerJoined event
        PlayerManager.instance.PlayerControllerJoined += OnPlayerControllerJoined;
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.PlayerControllerJoined -= OnPlayerControllerJoined;
        }

        if (lobbyLeaderInputModule != null)
        {
            // Unsubscribe from input actions
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

            // Proceed to accept input only from the lobby leader
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
            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();

            if (inputModule != null)
            {
                // Assign the PlayerInput to the UI Input Module
                lobbyLeaderController.playerInput.uiInputModule = inputModule;

                // Assign the lobby leader's actions asset to the input module
                inputModule.actionsAsset = lobbyLeaderController.playerInput.actions;

                // Subscribe to actions
                inputModule.submit.action.performed += OnSubmit;
                inputModule.cancel.action.performed += OnCancel;
                inputModule.move.action.performed += OnMove;

                lobbyLeaderInputModule = inputModule;
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

        // Load the next screen
        LoadGameCanvas();
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        // Handle the cancel action
        Debug.Log("Lobby Leader pressed Cancel.");
        // Implement your logic here, such as going back to the previous menu
    }

    void OnMove(InputAction.CallbackContext context)
    {
        // Handle the move action
        Vector2 movement = context.ReadValue<Vector2>();
        Debug.Log($"Lobby Leader moved: {movement}");
        // Implement your logic here, such as navigating the menu
    }

    void LoadGameCanvas()
    {
        // Deactivate the Main Menu Canvas
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Main Menu Canvas is not assigned in the LobbyManager.");
        }

        // Activate the Game Canvas
        if (gameCanvas != null)
        {
            gameCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Game Canvas is not assigned in the LobbyManager.");
        }

        // Optional: Re-enable player joining if you want other players to join on the next screen
        // PlayerManager.instance.SetJoiningEnabled(true);
    }

    // Optional: Method to access the lobby leader's controller from other scripts
    public PlayerController GetLobbyLeaderController()
    {
        return lobbyLeaderController;
    }
}
