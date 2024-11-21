using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class NPCMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] InputSystemUIInputModule _inputModule;
    [SerializeField] MultiplayerEventSystem multiplayerEventSystem;

    [Header("UIElements")]
    [SerializeField] Button _nextWaveButton;

    public bool secondLevel= false;
    public bool thirdLevel = false;

    public InputSystemUIInputModule inputModule => _inputModule;
    void Start()
    {
        if (WaveManager.instance is null) return;
        WaveManager.instance.gameStarting += WaveManager_GameStarted;
        WaveManager.instance.waveStarted += WaveManager_WaveStarted;
    }

    private void OnDestroy()
    {
        WaveManager.instance.gameStarting -= WaveManager_GameStarted;
        WaveManager.instance.waveStarted -= WaveManager_WaveStarted;
    }

    private void WaveManager_GameStarted(object sender, GameStartedEventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void ExitNPC()
    {
        multiplayerEventSystem.SetSelectedGameObject(null);
        Time.timeScale = 1;
    }

    private void EnterNPC()
    {
        // I commented this out because it wasn't working
        //_player.owningController.playerInput.uiInputModule = _inputModule;
        if (_usingMK == false)
            multiplayerEventSystem.SetSelectedGameObject(_nextWaveButton.gameObject);
        Time.timeScale = 0;
    }

    public void ConfigurePlayer(Player player)
    {
        _player = player;
        _usingMK = player.owningController.usingMK;
    }

    public void ToggleNPCUI(bool isEnabled)
    {
        gameObject?.SetActive(isEnabled);
        if (isEnabled)
        {
            EnterNPC();
        }
        else
        {
            ExitNPC();
        }
    }

    public void skipShopPhase()
    {
        if (WaveManager.instance is null) return;
        ExitNPC();
        WaveManager.instance.SkipShopPhase();
    }

    Player _player;
    bool _usingMK;
}