using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private CharacterCardController playerSetupMenu;
    [SerializeField] private GameObject playerCardRoot;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private Button startButton;
    [SerializeField] private string sceneToLoad = "Game";

    private Dictionary<int, bool> _readyStatusByPlayerId = new Dictionary<int, bool>();

    private void Awake()
    {
        startButton.gameObject.SetActive(true);
        startButton.interactable = false;
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void Start()
    {
        PlayerManager.instance.SetJoiningEnabled(true);
        PlayerManager.instance.PlayerControllerJoined += PlayerManager_OnPlayerControllerJoined;
        PlayerManager.instance.PlayerControllerRemoved += PlayerManager_OnPlayerControllerRemoved;
    }

    private void OnDestroy()
    {
        PlayerManager.instance.PlayerControllerJoined -= PlayerManager_OnPlayerControllerJoined;
        PlayerManager.instance.PlayerControllerRemoved -= PlayerManager_OnPlayerControllerRemoved;
    }

    private void PlayerManager_OnPlayerControllerJoined(object sender, PlayerManager.PlayerJoinedEventArgs e)
    {
        var playerController = e.playerController;
        var newCard = Instantiate(playerSetupMenu, playerCardRoot.transform);
        newCard.SetPlayerIndex(playerController.playerIndex);
        newCard.PlayerReadyStatusChanged += CharacterCardController_OnPlayerReadyStatusChanged;
        _readyStatusByPlayerId[playerController.playerIndex] = false;
        CheckIfShouldEnableStartButton();
    }

    private void PlayerManager_OnPlayerControllerRemoved(object sender, PlayerManager.PlayerRemovedEventArgs e)
    {
        _readyStatusByPlayerId.Remove(e.playerIndex);
        CheckIfShouldEnableStartButton();
    }

    private void CharacterCardController_OnPlayerReadyStatusChanged(object sender, CharacterCardController.PlayerReadyEventArgs e)
    {
        _readyStatusByPlayerId[e.playerId] = e.ready;
        CheckIfShouldEnableStartButton();
    }

    private void CheckIfShouldEnableStartButton()
    {
        bool allPlayersReady = _readyStatusByPlayerId.Count > 0 && _readyStatusByPlayerId.Values.All(status => status);
        startButton.interactable = allPlayersReady;
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
