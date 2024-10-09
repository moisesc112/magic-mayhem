using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class CharacterCardController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject readyIcon;
    [SerializeField] private Button readyButton;
    [SerializeField] private Image characterImage; // The Image component that will display the sprites
    [SerializeField] private Sprite defaultSprite; // The sprite shown before the player clicks "Ready"
    [SerializeField] private Sprite readySprite; // The sprite shown when the player is ready
    [SerializeField] private InputSystemUIInputModule _inputModule;

    private TextMeshProUGUI readyButtonText;
    private int _playerIndex;
    private bool _allowInput;
    private bool _isReady;
    private bool _buttonCooldown; // Variable to track if the button is in cooldown state

    public event EventHandler<PlayerReadyEventArgs> PlayerReadyStatusChanged;

    void Start()
    {
        if (readyButton == null)
        {
            Debug.LogError("ReadyButton is not assigned in the inspector.");
            return;
        }

        readyButtonText = readyButton.GetComponentInChildren<TextMeshProUGUI>();

        if (readyButtonText == null)
        {
            Debug.LogError("TextMeshProUGUI component is missing in the ReadyButton.");
            return;
        }

        readyButtonText.text = "Ready?";
        readyButtonText.alignment = TextAlignmentOptions.Center;

        // Set the default sprite initially
        characterImage.sprite = defaultSprite;

        // Remove any existing listeners and add the new listener
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(ToggleReadyState);

        StartCoroutine(nameof(WaitForInput));
        _inputModule.cancel.action.performed += OnCancel;

        Debug.Log("ReadyButton and ReadyButtonText are set up correctly.");
    }

    public void ToggleReadyState()
    {
        // Prevent multiple rapid presses using a cooldown
        if (_buttonCooldown)
        {
            Debug.Log($"Button is in cooldown for player {_playerIndex}. Ignoring click.");
            return;
        }

        Debug.Log($"Ready button clicked for player {_playerIndex}.");

        if (!_allowInput)
        {
            Debug.Log($"Input not allowed yet for player {_playerIndex}");
            return;
        }

        // Activate cooldown for a short duration
        StartCoroutine(ButtonCooldownCoroutine());

        // Toggle the ready state
        _isReady = !_isReady;
        readyIcon.SetActive(_isReady);

        // Swap the sprite based on the ready state
        characterImage.sprite = _isReady ? readySprite : defaultSprite;

        string newText = _isReady ? "Ready!" : "Ready?";
        readyButtonText.text = newText;
        readyButtonText.ForceMeshUpdate(); // Force the text to update in the UI

        Debug.Log($"Text is now: {readyButtonText.text}");

        PlayerReadyStatusChanged?.Invoke(this, new PlayerReadyEventArgs(_isReady, _playerIndex));
    }

    // Coroutine to handle a short cooldown for button presses
    private IEnumerator ButtonCooldownCoroutine()
    {
        _buttonCooldown = true;
        yield return new WaitForSeconds(0.1f); // Adjust this time if necessary
        _buttonCooldown = false;
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (_isReady)
            ToggleReadyState();
        else
            PlayerManager.instance.RemovePlayer(_playerIndex);

        Debug.Log($"Player {_playerIndex} pressed cancel.");
    }

    private IEnumerator WaitForInput()
    {
        yield return new WaitForSeconds(0.1f);
        _allowInput = true;
        Debug.Log($"Input is now allowed for player {_playerIndex}");
    }

    public void SetPlayerIndex(int index)
    {
        _playerIndex = index;
        titleText.text = $"Player {(_playerIndex + 1)}";
        Debug.Log($"Player index set to {_playerIndex}");
    }

    public class PlayerReadyEventArgs : EventArgs
    {
        public PlayerReadyEventArgs(bool isReady, int id)
        {
            ready = isReady;
            playerId = id;
        }

        public bool ready { get; private set; }
        public int playerId { get; private set; }
    }
}
