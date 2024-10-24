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

	// Use similar fields to the shop ui
	[SerializeField] MultiplayerEventSystem multiplayerEventSystem;
	[SerializeField] GameObject firstSelectedGameObject;
	[SerializeField] InputSystemUIInputModule _inputModule;
	public InputSystemUIInputModule inputSystemUIInputModule => _inputModule;

	void Awake()
	{
		// initialize with everything turned off
		loseText.gameObject.SetActive(false);
		winText.gameObject.SetActive(false);
		menuPanel.SetActive(false);
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
		// Clean up persisting game objects before we reset to the main menu
		GameObject debugUtil = GameObject.Find("UiDebugUtil");
		if (debugUtil != null)
		{
			Destroy(debugUtil);
		}

		GameObject[] playerControllerClones = GameObject.FindGameObjectsWithTag("PlayerControllerClone");
		foreach (GameObject clone in playerControllerClones)
		{
			Destroy(clone);
		}

		if (PlayerManager.instance != null)
		{
			Destroy(PlayerManager.instance.gameObject);
		}

		// reload the menu scene and reset time
		SceneManager.LoadScene("Menu", LoadSceneMode.Single);
		Time.timeScale = 1f;
	}

	public void LoseGameMenu()
    {
		gameOver = true;
		ToggleInGameMenuUI(true);
		loseText.gameObject.SetActive(true);
	}

	public void WinGameMenu()
	{
		gameOver = true;
		ToggleInGameMenuUI(true);
		winText.gameObject.SetActive(true);
	}

	public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
