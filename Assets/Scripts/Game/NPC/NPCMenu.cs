using UnityEngine;
using UnityEngine.UI;

public class NPCMenu : MonoBehaviour
{
    [Header("UIElements")]
    [SerializeField] Button _closeDialogButton;

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
		PlayerManager.instance.EnableAllMovement();
        PlayerManager.instance.ClearAllUIRoots();
		Time.timeScale = 1;
    }

    private void EnterNPC(PlayerController callingController)
    {
		var eventSystem = PlayerManager.instance.GetEventSystemForController(callingController);
        eventSystem.playerRoot = gameObject;
		eventSystem.SetSelectedGameObject(callingController.usingMK ? null : _closeDialogButton.gameObject);

        var inputModule = PlayerManager.instance.GetInputModuleForController(callingController);
        inputModule.enabled = true;

        PlayerManager.instance.DisableAllMovement();

		Time.timeScale = 0;
    }

    public void ToggleNPCUI(bool isEnabled, PlayerController callingController)
    {
        gameObject?.SetActive(isEnabled);
        if (isEnabled && callingController is object)
        {
            EnterNPC(callingController);
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
        PlayerManager.instance.ClearAllTextPrompts();
        WaveManager.instance.SkipShopPhase();
    }

    public void CloseDialog()
    {
        ToggleNPCUI(false, null);
    }
}