using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class AbilitySlotConfirmationController : MonoBehaviour
{
    [SerializeField] InputSystemUIInputModule _inputModule;
    [SerializeField] Player _player;
    [SerializeField] MultiplayerEventSystem multiplayerEventSystem;
    [SerializeField] GameObject firstSelectedGameObject;

    private void Awake()
    {
        var playerController = PlayerManager.instance.PlayerControllers.FirstOrDefault(x => x.playerIndex == _player.GetPlayerIndex());
        if (playerController != null)
        {
            playerController.playerInput.uiInputModule = _inputModule;
        }
    }

    public void UpdateAbilityOptionDisplay()
    {
        // TODO display ability current icons and names

        multiplayerEventSystem.SetSelectedGameObject(firstSelectedGameObject);
    }
}
