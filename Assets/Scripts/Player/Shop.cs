using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<AbilityInfo> abilityRegistry;
    public List<AbilityInfo> currentAbilitiesInShop;

    [SerializeField] Player player;
    [SerializeField] GameObject shopUI;
    [SerializeField] GameObject abilitySlotConfirmation;

    public void SetAbilityToAdd(AbilityInfo ability) => _abilityToAdd = ability;
    private AbilityInfo _abilityToAdd;

    private void Awake()
    {
        shopUI.SetActive(false);
        abilitySlotConfirmation.SetActive(false);
    }

    private void Start()
    {
        ShuffleShopAbilityOptions();
    }

    public void ShuffleShopAbilityOptions()
    {
        currentAbilitiesInShop = new List<AbilityInfo>();

        var abilityShopOptions = 3;
        var currentRandomChoices = new List<int>();
        for(int i = 0; i < abilityShopOptions; i++)
        {
            var randomChoice = Random.Range(0, abilityRegistry.Count);
            while (currentRandomChoices.Contains(randomChoice))
            {
                randomChoice = Random.Range(0, abilityRegistry.Count);
            }
            currentRandomChoices.Add(randomChoice);
        }

        foreach(var choice in currentRandomChoices)
        {
            currentAbilitiesInShop.Add(abilityRegistry[choice]);
        }
        shopUI.GetComponent<ShopUIController>().UpdateShopDisplay(currentAbilitiesInShop, false);
    }

    public void PurchaseAbility(int abilityOption)
    {
        if (currentAbilitiesInShop[abilityOption].cost > player.PlayerStats.gold)
        {
            // TODO show a message to player?
            return;
        }

        SetAbilityToAdd(currentAbilitiesInShop[abilityOption]);
        Debug.Log($"Ability Purchased! {_abilityToAdd.name}");
        shopUI.SetActive(false);
        abilitySlotConfirmation.SetActive(true);
        abilitySlotConfirmation.GetComponent<AbilitySlotConfirmationController>().UpdateAbilityOptionDisplay();
    }

    public void SetPurchasedAbilitySlot(int slotNumber)
    {
        if (_abilityToAdd != null)
        {
            var abilitySlotComponent = player.GetComponentInChildren<AbilitySlotsComponent>();
            if (abilitySlotComponent != null)
            {
                player.PlayerStats.gold -= _abilityToAdd.cost;
                abilitySlotComponent.UpdateAbilitySlot(_abilityToAdd, slotNumber);
                Debug.Log($"Ability {_abilityToAdd.name} added to slot number {slotNumber}!");
            }
        }
        _abilityToAdd = null;
        abilitySlotConfirmation.SetActive(false);
        shopUI.SetActive(true);
        shopUI.GetComponent<ShopUIController>().UpdateShopDisplay(currentAbilitiesInShop);
    }

    public void ToggleShopUI(bool isEnabled)
    {
        abilitySlotConfirmation.SetActive(false);
        shopUI.SetActive(isEnabled);
        shopUI.GetComponent<ShopUIController>().UpdateShopDisplay(currentAbilitiesInShop);
    }
}
