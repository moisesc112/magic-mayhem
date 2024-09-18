using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public List<AbilityInfo> abilityRegistry;

    [SerializeField]
    private TextMeshProUGUI abilityOption1;
    [SerializeField]
    private TextMeshProUGUI abilityOption2;
    [SerializeField]
    private TextMeshProUGUI abilityOption3;

    private List<AbilityInfo> currentAbilitiesInShop;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        ShuffleShopAbilityOptions();
        UpdateShopDisplay();
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

    public void UpdateShopDisplay()
    {
        if (currentAbilitiesInShop != null && currentAbilitiesInShop.Count() >= 3)
        {
            abilityOption1.text = currentAbilitiesInShop[0].name;
            abilityOption2.text = currentAbilitiesInShop[1].name;
            abilityOption3.text = currentAbilitiesInShop[2].name;
        }
    }

    public void PurchaseAbility(int abilityIndex)
    {
        
    }

    public void ToggleShopUI(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }
}
