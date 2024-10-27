using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] GameObject playerCardRoot;
	[SerializeField] TextMeshProUGUI countDownText;
	[SerializeField] string sceneToLoad;
	[SerializeField] Animator _cameraAnim;

	[Header("MenuSections")]
	[SerializeField] GameObject _startSection;
	[SerializeField] GameObject _lobbySection;

	private void Awake()
	{
		_playerControllerByIndex = new Dictionary<int, PlayerController>();
		_lobbyEventSystem = _lobbySection.GetComponent<EventSystem>();
		_lobbyInputSystem = _lobbySection.GetComponent<InputSystemUIInputModule>();
	}

	void Start()
	{
		PlayerManager.instance.SetJoiningEnabled(true);
		PlayerManager.instance.PlayerControllerJoined += PlayerManager_OnPlayerControllerJoined;
		PlayerManager.instance.PlayerControllerRemoved += PlayerManager_OnPlayerControllerRemoved;

		LevelLoadManager.instance.LoadSceneAsync(sceneToLoad);
		LevelLoadManager.instance.sceneLoaded += LevelManager_OnSceneLoaded;
		_menuCharacters = GameObject.FindGameObjectsWithTag("MenuCharacter").Select(c => c.GetComponent<MenuCharacter>()).ToArray();
	}

	void OnDestroy()
	{
		PlayerManager.instance.PlayerControllerJoined -= PlayerManager_OnPlayerControllerJoined;
	}

	public void StartGame()
	{
		PlayerManager.instance.SetJoiningEnabled(false);
		LevelLoadManager.instance.ActivateLoadedScene();	
	}

	public void StartTutorial()
	{
		SceneManager.LoadScene("Tutorial Level");
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	void LevelManager_OnSceneLoaded(object sender, LevelLoadedArgs args)
	{
		LevelLoadManager.instance.sceneLoaded -= LevelManager_OnSceneLoaded;
	}

	void PlayerManager_OnPlayerControllerJoined(object sender, PlayerManager.PlayerJoinedEventArgs e)
	{
		var playerController = e.playerController;

		var matchingCharacter = _menuCharacters.First(c => c.playerIndex == e.playerController.playerIndex);
		if (matchingCharacter is null)
		{
			Debug.LogError($"ERROR: Unable to find matching player character for player {e.playerController.playerIndex}.");
			return;
		}

		_playerControllerByIndex[e.playerController.playerIndex] = e.playerController;
		playerController.playerInput.uiInputModule = matchingCharacter.inputModule;
		matchingCharacter.Join();

		if (!_playerLoaded)
		{
			_playerLoaded = true;
			_cameraAnim.SetTrigger("ToCharacters");
			_startSection.SetActive(false);
			_lobbySection.SetActive(true);
		}
	}

	void PlayerManager_OnPlayerControllerRemoved(object sender, PlayerManager.PlayerRemovedEventArgs e)
	{
		_playerControllerByIndex.Remove(e.playerIndex);
		_menuCharacters.First(c => c.playerIndex == e.playerIndex).BackOut();

		if (_playerControllerByIndex.Count == 0)
		{
			_playerLoaded = false;
			_cameraAnim.SetTrigger("ToBase");
			_startSection.SetActive(true);
			_lobbySection.SetActive(false);
		}
	}

	Dictionary<int, PlayerController> _playerControllerByIndex;
	EventSystem _lobbyEventSystem;
	InputSystemUIInputModule _lobbyInputSystem;
	MenuCharacter[] _menuCharacters;

	bool _playerLoaded = false;
}
