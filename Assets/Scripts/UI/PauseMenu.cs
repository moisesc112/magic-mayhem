using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] InputSystemUIInputModule _inputModule;
	[SerializeField] MultiplayerEventSystem _multiplayerEventSystem;
	[SerializeField] GameObject _firstSelectedGameObject;

	[Header("UI References")]
	[SerializeField] Image _frame;
	[SerializeField] GameObject _pausePanel;
	[SerializeField] GameObject _winText;
	[SerializeField] GameObject _loseText;

	public void ConfigurePlayer(Player player)
	{
		_player = player;
		_usingMK = player.owningController.usingMK;
		_frame.color = player.playerColor;
	}

	public void OpenWinMenu()
	{
		_loseText.SetActive(false);
		_winText.SetActive(true);
		OpenPauseMenu();
	}

	public void OpenLoseMenu()
	{
		_loseText.SetActive(true);
		_winText.SetActive(false);
		OpenPauseMenu();
	}

	public void OpenPauseMenu()
	{
		_player.owningController.playerInput.uiInputModule = _inputModule;
		_player.owningController.DisableMovement();
		_multiplayerEventSystem.SetSelectedGameObject(null);
		if (_usingMK == false)
			_multiplayerEventSystem.SetSelectedGameObject(_firstSelectedGameObject);
		_pausePanel.SetActive(true);
		Time.timeScale = 0;
	}

	public void ClosePauseMenu()
	{
		_pausePanel.SetActive(false);
		Time.timeScale = 1;
	}

	Player _player;
	bool _usingMK;
}
