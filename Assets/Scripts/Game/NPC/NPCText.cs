using TMPro;
using UnityEngine;

public class NPCText : MonoBehaviour
{
    public TextMeshProUGUI infoText;

    // Start is called before the first frame update
    void Start()
    {
        infoText.text = firstStage;
    }

    public void UpdateText(string newText)
    {
        infoText.text = newText;
    }

    private string firstStage = "Traveler, the goblins are coming! Survive their onslaught for ten days, and use the time in between to buy more powerful spells from the shop! There are vases that contain gold, barrels with health potions  and spike traps scattered around the map to aid you. Are you ready wield magic and ensure your survival!";
    private string secondStage = "aghhhh";
}
