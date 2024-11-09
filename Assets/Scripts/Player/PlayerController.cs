using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
	public int playerIndex => _playerInput.playerIndex;
	public PlayerInput playerInput => _playerInput;
	public Player player => _player;

	private bool playerInShop;

	void Awake()
	{
		_playerInput = GetComponent<PlayerInput>();
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		PlayerManager.instance.RegisterPlayer(this);
	}
	
	public void TakeControl(Player player)
	{
		_player = player;
		_player.Possess(this);
		_playerInput.camera = _player.playerCamera;
	}

	public void ReleaseControl()
	{
		if (_player is object)
			_player.Release();
		_player = null;
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		var moveInput = context.ReadValue<Vector2>();
		if (_player)
			_player.MovePlayer(moveInput);
	}

	public void OnCastSpell(InputAction.CallbackContext context)
	{
		if (_player)
			_player.UpdateCasting(context.performed);

	}

	public void OnActivateTrap(InputAction.CallbackContext context)
	{
		if(context.performed && _player)
			_player.ActivateTrap(true);
	}

	public void OnToggleShopUI(InputAction.CallbackContext context)
	{
		if (_player && context.performed && _player.playerInShopRange)
		{
			_player.ToggleShopUI(true);
			_playerInput.actions.FindActionMap("Gameplay").Disable();
			_playerInput.actions.FindActionMap("UI").Enable();
			_playerInput.actions.FindAction("CloseGameMenu").Disable();
			_playerInput.actions.FindAction("OpenMenu").Enable();
			playerInShop = true;
		}
	}

	public void OnToggleInGameMenuUI(InputAction.CallbackContext context)
	{
		if (_player && context.performed)
		{
			_player.ToggleInGameMenuUI(true);
			_playerInput.actions.FindActionMap("Gameplay").Disable();
			_playerInput.actions.FindActionMap("UI").Enable();
			_playerInput.actions.FindAction("CloseShopUI").Disable();
			if (playerInShop)
            {
				_player.ToggleShopUI(false);
			}
		}
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		if (_player)
		{
			var dir = Vector2.zero;
			if (_playerInput.currentControlScheme == c_gamepadScheme)
				dir = context.ReadValue<Vector2>();
			_player.SetAiming(context.performed, dir, useMouse: _playerInput.currentControlScheme == c_keyboardScheme);
		}
	}

	public void OnSelectAbility(int slotNumber)
	{
		if (_player)
			_player.SelectAbility(slotNumber);
	}

    public void OnSelectAbilityLeft(InputAction.CallbackContext context)
    {
        if (_player && context.performed)
            _player.SelectAbilityByDirection(SelectAbilityDirection.Left);
    }

    public void OnSelectAbilityRight(InputAction.CallbackContext context)
    {
        if (_player && context.performed)
            _player.SelectAbilityByDirection(SelectAbilityDirection.Right);
    }

	public void OnScrollAbilityLeft(InputAction.CallbackContext context)
	{
		if (_player && context.performed)
			_player.SelectAbilityByDirection(SelectAbilityDirection.Left);
	}

    public void OnScrollAbilityRight(InputAction.CallbackContext context)
    {
        if (_player && context.performed)
            _player.SelectAbilityByDirection(SelectAbilityDirection.Right);
    }

    public void OnRoll()
	{
		if (_player)
			_player.OnRoll();
	}

	public void OnDebug()
	{
		Debug.Log("hit");
	}

	public void OnCloseShopUI(InputAction.CallbackContext context)
	{
		_playerInput.actions.FindActionMap("UI").Disable();
		_playerInput.actions.FindActionMap("Gameplay").Enable();
		_player.ToggleShopUI(false);
		playerInShop = false;
	}
	
	public void ForceCloseActiveShopUI(PlayerController controller)
	{
		controller.playerInput.actions.FindActionMap("UI").Disable();
		controller.playerInput.actions.FindActionMap("Gameplay").Enable();
		_player.ToggleShopUI(false);
		controller.playerInput.actions.FindAction("OpenShop").Disable();
		playerInShop = false;
	}
	
	public void OnCloseInGameMenuUI(InputAction.CallbackContext context)
	{	if (_player.GameOver())
		{
			return;
		}
		_playerInput.actions.FindActionMap("UI").Disable();
		_playerInput.actions.FindActionMap("Gameplay").Enable();
		_player.ToggleInGameMenuUI(false);
		if (WaveManager.instance!= null && !WaveManager.instance.inWaveCooldown)
		{
			_playerInput.actions.FindAction("OpenShop").Disable();
		}
	}

	public void OnDeviceLost(PlayerInput lostPlayer)
	{
		Debug.Log($"Player {lostPlayer.playerIndex} was lost.");
		PlayerManager.instance.OnPlayerLeft(this);
	}

	public void OnDeviceRegained(PlayerInput player)
	{
		Debug.Log($"Player {player.playerIndex} has rejoined!");
		PlayerManager.instance.OnPlayerRejoined(this);
	}

	public void SetUpInputCamera() => _playerInput.camera = _player.playerCamera;

	const string c_gamepadScheme = "Gamepad";
	const string c_keyboardScheme = "MK";

	Player _player;
	PlayerInput _playerInput;
}
