using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Unity.VisualScripting;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;


public class InGameMenu : MonoBehaviour
{
	public Button ResetGame;
	public Button Quit;
	public TextMeshProUGUI loseText;
	public TextMeshProUGUI winText;
	public GameObject menuPanel;
	public bool gameOver { get; private set; } = false;

	[SerializeField] MultiplayerEventSystem multiplayerEventSystem;
	[SerializeField] GameObject firstSelectedGameObject;
	[SerializeField] InputSystemUIInputModule _inputModule;
	[SerializeField] Player _player;
	public InputSystemUIInputModule inputSystemUIInputModule => _inputModule;

	void Awake()
	{
		loseText.gameObject.SetActive(false);
		winText.gameObject.SetActive(false);
		menuPanel.SetActive(false);

		var playerController = PlayerManager.instance.PlayerControllers.FirstOrDefault(x => x.playerIndex == _player.GetPlayerIndex());
		if (playerController != null)
		{
			playerController.playerInput.uiInputModule = _inputModule;
		}
	}

	public void UpdateMenuDisplay(bool setFirstSelectedGameObject = true)
    {
		if (setFirstSelectedGameObject)
		{
			multiplayerEventSystem.SetSelectedGameObject(firstSelectedGameObject);
		}
	}

	public void ToggleInGameMenuUI(bool isEnabled)
    {
		menuPanel.SetActive(isEnabled);
		Time.timeScale = isEnabled ? 0 : 1;
    }

	public void RestartGame()
	{
		SceneManager.LoadScene("Menu");
	}

	public void LoseGameMenu()
    {
		gameOver = true;
		ToggleInGameMenuUI(true);
		loseText.gameObject.SetActive(true);
	}

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

	public Player GetUIControllingPlayer() => _player;
}
