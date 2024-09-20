using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public List<AbilityInfo> abilityRegistry;
    public List<AbilityInfo> currentAbilitiesInShop;


    private void Awake()
    {
        _shopUIController = GetComponent<ShopUIController>();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        ShuffleShopAbilityOptions();
        _shopUIController.UpdateShopDisplay(currentAbilitiesInShop);
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
    }

    public void PurchaseAbility(int abilityOption)
    {
        Debug.LogWarning($"Ability Purchased! {currentAbilitiesInShop[abilityOption].name}");
    }

    public void ToggleShopUI(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }

    private ShopUIController _shopUIController;
}
