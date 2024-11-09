using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI goldText;

    private void Update()
    {
        UpdateHealthUI();
        goldText.text = playerStats.gold.ToString();
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = playerStats.health / playerStats.maxHealth;
        healthText.text = $"{playerStats.health.ToString("0.0")} / {playerStats.maxHealth.ToString("#.0")}";
    }
}
