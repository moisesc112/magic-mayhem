using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class InGameMenu : MonoBehaviour
{

	public Button ResetGame;
	public Button Quit;
	public TextMeshProUGUI loseText;
	public TextMeshProUGUI winText;
	public GameObject menuPanel;
	private bool isPaused = false;

	void Start()
	{
		loseText.gameObject.SetActive(false);
		winText.gameObject.SetActive(false);
		menuPanel.SetActive(false);
	}

	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleInGameMenu();
        }
    }

	void ToggleInGameMenu()
    {
		isPaused = !isPaused;
		menuPanel.SetActive(isPaused);
		Time.timeScale = isPaused ? 0 : 1;
    }

	public void RestartGame()
	{
		SceneManager.LoadScene("Menu");
	}

	public void LoseGameMenu()
    {
		ToggleInGameMenu();
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

}
