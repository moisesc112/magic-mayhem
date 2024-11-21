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
    public bool secondLevel= false;
    public bool thirdLevel = false;

    public InputSystemUIInputModule inputModule => _inputModule;

    // Start is called before the first frame update
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

    private void CloseNPC()
    {
        multiplayerEventSystem.SetSelectedGameObject(null);
    }

    private void OpenShop()
    {
    }

    public void ToggleNPC(bool isEnabled)
    {
        _player.owningController.playerInput.uiInputModule = _inputModule;
        gameObject?.SetActive(isEnabled);
        if (isEnabled)
        {
            OpenShop();
        }
        else
        {
            CloseNPC();
        }
    }

    public void skipShopPhase()
    {
        if (WaveManager.instance is null) return;
        WaveManager.instance.SkipShopPhase();
    }

    Player _player;
}