using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] Button abilityOptionButton1;
    [SerializeField] Button abilityOptionButton2;
    [SerializeField] Button abilityOptionButton3;

    [SerializeField] TextMeshProUGUI abilityOption1;
    [SerializeField] TextMeshProUGUI abilityOption2;
    [SerializeField] TextMeshProUGUI abilityOption3;

    [SerializeField] GameObject firstSelectedGameObject;
    public InputSystemUIInputModule inputSystemUIInputModule => _inputModule;
    [SerializeField] InputSystemUIInputModule _inputModule;
    [SerializeField] Player _player;

    private void Awake()
    {
        var playerController = PlayerManager.instance.PlayerControllers.First(x => x.playerIndex == _player.GetPlayerIndex());
        if (playerController != null)
        {
            playerController.playerInput.uiInputModule = _inputModule;
        }
    }

    public void UpdateShopDisplay(List<AbilityInfo> currentAbilitiesInShop)
    {
        if (currentAbilitiesInShop != null && currentAbilitiesInShop.Count() >= 3)
        {
            abilityOption1.text = currentAbilitiesInShop[0].name;
            abilityOption2.text = currentAbilitiesInShop[1].name;
            abilityOption3.text = currentAbilitiesInShop[2].name;
        }
    }
}
