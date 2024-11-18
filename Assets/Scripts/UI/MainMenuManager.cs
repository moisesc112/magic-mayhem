using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] string sceneToLoad;
	[SerializeField] Animator _cameraAnim;

	[Header("MenuSections")]
	[SerializeField] GameObject _startSection;
	[SerializeField] GameObject _lobbySection;

	[Header("UIReferences")]
	[SerializeField] GameObject _uiFirstSelected;

	private void Awake()
	{
		_playerControllerByIndex = new Dictionary<int, PlayerController>();
		_lobbyEventSystem = _lobbySection.GetComponent<EventSystem>();
		_lobbyInputSystem = _lobbySection.GetComponent<InputSystemUIInputModule>();
		_multiplayerEventSystem = _lobbySection.GetComponent<MultiplayerEventSystem>();
	}

	void Start()
	{
		PlayerManager.instance.SetJoiningEnabled(true);
		PlayerManager.instance.PlayerControllerJoined += PlayerManager_OnPlayerControllerJoined;
		PlayerManager.instance.PlayerControllerRemoved += PlayerManager_OnPlayerControllerRemoved;

		LevelLoadManager.instance.LoadSceneAsync(sceneToLoad);
		LevelLoadManager.instance.sceneLoaded += LevelManager_OnSceneLoaded;
		_menuCharacters = GameObject.FindGameObjectsWithTag("MenuCharacter").Select(c => c.GetComponent<MenuCharacter>()).ToArray();

		StartCoroutine(nameof(IncreaseMusicIntensity));
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

	void PlayerManager_OnPlayerControllerJoined(object sender, GenericEventArgs<PlayerController> e)
	{
		var playerController = e.value;

		var matchingCharacter = _menuCharacters.First(c => c.playerIndex == playerController.playerIndex);
		if (matchingCharacter is null)
		{
			Debug.LogError($"ERROR: Unable to find matching player character for player {playerController.playerIndex}.");
			return;
		}

		_playerControllerByIndex[playerController.playerIndex] = playerController;
		playerController.playerInput.uiInputModule = matchingCharacter.inputModule;
		matchingCharacter.Join();

		if (!_hostLoaded)
		{
			_hostLoaded = true;
			_lobbyHostIndex = playerController.playerIndex;
			_cameraAnim.SetTrigger("ToCharacters");
			_startSection.SetActive(false);
			_lobbySection.SetActive(true);
			SetUIControlSchemeFromController(playerController);
		}
	}

	void PlayerManager_OnPlayerControllerRemoved(object sender, GenericEventArgs<int> e)
	{
		var playerIndex = e.value;
		_playerControllerByIndex.Remove(playerIndex);
		_menuCharacters.First(c => c.playerIndex == playerIndex).BackOut();

		if (_playerControllerByIndex.Count == 0)
		{
			_hostLoaded = false;
			_cameraAnim.SetTrigger("ToBase");
			_startSection.SetActive(true);
			_lobbySection.SetActive(false);
		}
		else if (playerIndex == _lobbyHostIndex)
		{
			SetUIControlSchemeFromController(_playerControllerByIndex.Values.First());
		}
	}

	void SetUIControlSchemeFromController(PlayerController controller)
	{
		_multiplayerEventSystem.SetSelectedGameObject(controller.usingMK ? null : _uiFirstSelected);
	}

	IEnumerator IncreaseMusicIntensity()
	{
		yield return new WaitForSeconds(0.5f);
		SimpleAudioManager.Manager.instance.SetIntensity(0);
		yield return new WaitForSeconds(15.0f);
		SimpleAudioManager.Manager.instance.SetIntensity(1);
		yield return new WaitForSeconds(15.0f);
		SimpleAudioManager.Manager.instance.SetIntensity(2);
	}

	Dictionary<int, PlayerController> _playerControllerByIndex;
	EventSystem _lobbyEventSystem;
	InputSystemUIInputModule _lobbyInputSystem;
	MenuCharacter[] _menuCharacters;
	MultiplayerEventSystem _multiplayerEventSystem;
	int _lobbyHostIndex;

	bool _hostLoaded = false;
}
