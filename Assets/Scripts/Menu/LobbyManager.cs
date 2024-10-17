using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using System.Collections; // Added for Coroutines

public class LobbyManager : MonoBehaviour
{
    private PlayerController lobbyLeaderController;
    private InputSystemUIInputModule lobbyLeaderInputModule;
    private bool isTransitioning = false; // Flag to prevent overlapping transitions

    [Header("UI Canvases")]
    public GameObject mainMenuCanvas;
    public GameObject gameCanvas;
    public GameObject menuCanvas; 

    [Header("Camera Settings")]
    public Camera mainCamera;

    // Existing Camera Position and Rotation for GameCanvas
    public Vector3 gameCanvasCameraPosition = new Vector3(-970.83f, 302.02f, 391.857f);
    public Vector3 gameCanvasCameraRotation = new Vector3(1.179f, 10.548f, 2.118f);

    // New Camera Position and Rotation for Lobby
    [Header("Lobby Camera Settings")]
    public Vector3 lobbyCameraPosition = new Vector3(-1055.654f, 300.97f, 348.8f); // Set your desired Lobby position
    public Vector3 lobbyCameraRotation = new Vector3(-11.409f, 1.694f, 360.609f);   // Set your desired Lobby rotation

    // Duration for the smooth transition to Lobby (in seconds)
    [SerializeField]
    private float lobbyTransitionDuration = 2.0f;

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
        // Check if the menu is active
        if (menuCanvas != null && menuCanvas.activeInHierarchy)
        {
            // If the menu is active, do not process the Submit action here
            return;
        }

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
        // Check if the menu is active
        if (menuCanvas != null && menuCanvas.activeInHierarchy)
        {
            // If the menu is active, do not process the Move action here
            return;
        }

        // Handle the move action
        Vector2 movement = context.ReadValue<Vector2>();
        Debug.Log($"Lobby Leader moved: {movement}");
        // Implement your logic here
    }

    void MoveCameraToGameCanvas()
    {
        if (mainCamera != null)
        {
            if (mainMenuCanvas != null)
            {
                mainMenuCanvas.SetActive(false);
            }

            Vector3 cameraPosition = gameCanvasCameraPosition;
            Quaternion cameraRotation = Quaternion.Euler(gameCanvasCameraRotation);

            // Snap the camera to the new position and rotation
            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.rotation = cameraRotation;

            if (gameCanvas != null)
            {
                gameCanvas.SetActive(true);
            }

            Debug.Log("Camera snapped to GameCanvas.");
        }
        else
        {
            Debug.LogWarning("Main Camera is not assigned in the LobbyManager.");
        }
    }

    // Updated method to smoothly transition the camera to Lobby
    public void MoveCameraToLobby()
    {
        if (mainCamera != null && !isTransitioning)
        {
            Vector3 targetPosition = lobbyCameraPosition;
            Quaternion targetRotation = Quaternion.Euler(lobbyCameraRotation);

            // Start the smooth transition Coroutine
            StartCoroutine(SmoothTransition(targetPosition, targetRotation, lobbyTransitionDuration));

            Debug.Log("Camera is moving to Lobby.");
        }
        else
        {
            if (isTransitioning)
            {
                Debug.LogWarning("Camera transition is already in progress.");
            }
            else
            {
                Debug.LogWarning("Main Camera is not assigned in the LobbyManager.");
            }
        }
    }

    // Coroutine to handle the smooth camera transition
    private IEnumerator SmoothTransition(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        isTransitioning = true; // Set the flag to prevent overlapping transitions

        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Calculate the interpolation factor (0 to 1)
            float t = elapsed / duration;

            // Optional: Apply easing for smoother transition
            t = EaseInOutQuad(t);
            // t = EaseInOutCubic(t);


            // Smoothly interpolate position and rotation
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the camera reaches the exact target position and rotation
        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;

        isTransitioning = false; // Reset the flag

        Debug.Log("Camera movement to Lobby completed.");
    }

    // Optional: Easing function for smoother transitions
    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }

    // private float EaseInOutCubic(float t)
    // {
    //     return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3) / 2f;
    // }
    // Optional: Method to access the lobby leader's controller from other scripts
    public PlayerController GetLobbyLeaderController()
    {
        return lobbyLeaderController;
    }
}
