using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
	public int playerIndex => _playerInput.playerIndex;
	public PlayerInput playerInput => _playerInput;

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
		_player.SetPlayerIndex(_playerInput.playerIndex);
		_playerInput.camera = _player.playerCamera;
	}

	public void ReleaseControl()
	{
		if (_player is object)
			_player.SetPlayerIndex(-1);
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

	public void OnAim(InputAction.CallbackContext context)
	{
		if (_player)
			_player.SetAiming(context.performed);
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

	Player _player;
	PlayerInput _playerInput;
}
