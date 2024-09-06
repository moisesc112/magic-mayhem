using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] CharacterCardController playerSetupMenu;
	[SerializeField] GameObject playerCardRoot;
	[SerializeField] TextMeshProUGUI countDownText;
	[SerializeField] string sceneToLoad;

	private void Awake()
	{
		_playerCardByIndex = new Dictionary<int, CharacterCardController> ();
	}

	void Start()
	{
		PlayerManager.instance.SetJoiningEnabled(true);
		PlayerManager.instance.PlayerControllerJoined += PlayerManager_OnPlayerControllerJoined;
		PlayerManager.instance.PlayerControllerRemoved += PlayerManager_OnPlayerControllerRemoved;
	}

	void OnDestroy()
	{
		PlayerManager.instance.PlayerControllerJoined -= PlayerManager_OnPlayerControllerJoined;
	}

	public void StartGame()
	{
		PlayerManager.instance.SetJoiningEnabled(false);
	}

	void PlayerManager_OnPlayerControllerJoined(object sender, PlayerManager.PlayerJoinedEventArgs e)
	{
		var playerController = e.playerController;
		var newCard = Instantiate(playerSetupMenu, playerCardRoot.transform);
		newCard.SetPlayerIndex(playerController.playerIndex);
		newCard.PlayerReadyStatusChanged += CharacterCardController_OnPlayerReadyStatusChanged;
		playerController.playerInput.uiInputModule = newCard.inputSystemUIInputModule;
		_playerCardByIndex.Add(e.playerController.playerIndex, newCard);
		_totalPlayers++;
	}

	void PlayerManager_OnPlayerControllerRemoved(object sender, PlayerManager.PlayerRemovedEventArgs e)
	{
		if (!_playerCardByIndex.TryGetValue(e.playerIndex, out var cardController)) return;

		cardController.PlayerReadyStatusChanged -= CharacterCardController_OnPlayerReadyStatusChanged;

		Destroy(cardController.gameObject);
		_playerCardByIndex.Remove(e.playerIndex);
		_totalPlayers--;
	}

	private void CharacterCardController_OnPlayerReadyStatusChanged(object sender, CharacterCardController.PlayerReadyEventArgs e)
	{
		if (e.ready)
			_readyPlayers++;
		else
			_readyPlayers--;

		// This is hacky, maybe think of a better way in the future.
		if (_readyPlayers == _totalPlayers)
		{
			// Show countdown
			countDownText.gameObject.transform.parent.gameObject.SetActive(true);
			StartCoroutine(nameof(BeginCountdown));
		}
		else
		{
			// Hide countdown, stop timer
			countDownText.gameObject.transform.parent.gameObject.SetActive(false);
			StopCoroutine(nameof(BeginCountdown));
		}
	}

	IEnumerator BeginCountdown()
	{
		var count = 3;
		for(int i = count; i >= 0; i--)
		{
			countDownText.text = i.ToString();
			yield return new WaitForSeconds(1.0f);
		}

		SceneManager.LoadScene(sceneToLoad);
	}


	int _readyPlayers;
	int _totalPlayers;
	Dictionary<int, CharacterCardController> _playerCardByIndex;
}
