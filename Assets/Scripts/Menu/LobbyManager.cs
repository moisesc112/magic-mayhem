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

    [Header("Camera Settings")]
    public Camera mainCamera;
    // public float cameraMoveDuration = 1.0f; // No longer needed

    void Start()
    {
        // Enable player joining
        PlayerManager.instance.SetJoiningEnabled(true);

        // Subscribe to the PlayerControllerJoined event
        PlayerManager.instance.PlayerControllerJoined += OnPlayerControllerJoined;

        // Ensure the mainCamera is assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
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

            // Move the camera immediately after setting up the input
            MoveCameraToGameCanvas();
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

        // Move the camera to the GameCanvas location
        MoveCameraToGameCanvas();
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

    void MoveCameraToGameCanvas()
    {
        if (gameCanvas != null && mainCamera != null)
        {
            // Optionally deactivate the Main Menu Canvas
            if (mainMenuCanvas != null)
            {
                mainMenuCanvas.SetActive(false);
            }

            // Get the target position (assuming you want the camera to look at the canvas)
            Vector3 targetPosition = gameCanvas.transform.position;

            // Optionally adjust the camera's Z position or other offsets
            // For example, if you want the camera to be a certain distance away from the canvas
            targetPosition.z = mainCamera.transform.position.z;

            // Move the camera instantly to the target position
            mainCamera.transform.position = targetPosition;

            // Optional: Re-enable player joining if you want other players to join
            // PlayerManager.instance.SetJoiningEnabled(true);
        }
        else
        {
            if (gameCanvas == null)
                Debug.LogWarning("Game Canvas is not assigned in the LobbyManager.");
            if (mainCamera == null)
                Debug.LogWarning("Main Camera is not assigned in the LobbyManager.");
        }
    }

    // Optional: Remove or comment out the coroutine if it's no longer needed
    /*
    IEnumerator MoveCameraCoroutine(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = end;

        // Optionally, enable player joining after the camera movement is complete
        // PlayerManager.instance.SetJoiningEnabled(true);
    }
    */

    // Optional: Method to access the lobby leader's controller from other scripts
    public PlayerController GetLobbyLeaderController()
    {
        return lobbyLeaderController;
    }
}
