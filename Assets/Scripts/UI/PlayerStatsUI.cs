using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStatsUI : MonoBehaviour
{
    public PlayerStats playerStats;

    public Slider healthSlider;
    public Slider hungerSlider;
    public Slider thirstSlider;
    public Slider energySlider;

    void Start()
    {
        if (playerStats != null)
        {
            // Initialize sliders
            healthSlider.maxValue = playerStats.maxHealth;
            hungerSlider.maxValue = playerStats.maxHunger;
            thirstSlider.maxValue = playerStats.maxThirst;
            energySlider.maxValue = playerStats.maxEnergy;

            // Set initial values
            UpdateUI();

            // Start coroutine to continuously update UI
            StartCoroutine(UpdateUICoroutine());
        }
        else
        {
            Debug.LogError("PlayerStats reference is missing in PlayerStatsUI.");
        }
    }

    IEnumerator UpdateUICoroutine()
    {
        while (true)
        {
            UpdateUI();
            yield return new WaitForSeconds(0.5f); // Update UI every half a second
        }
    }

    void UpdateUI()
    {
        // Update the UI sliders to match the current player stats
        healthSlider.value = playerStats.currentHealth;
        hungerSlider.value = playerStats.currentHunger;
        thirstSlider.value = playerStats.currentThirst;
        energySlider.value = playerStats.currentEnergy;
    }
}