using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI dmgText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI coinsText;

    [SerializeField] private PlayerCombat playerStats;
    private int coins;

    // Handle cursor state
    private CursorLockMode previousLockState;
    private bool previousCursorVisibility;
    public GameObject panel;
    private void Start()
    {
        coins = playerStats.stats.coins;
        // Update UI with current values
        UpdateUI();
    }
    private void Update()
    {
        UpdateUI();
    }
    public void IncreaseHP()
    {
        if (coins >= 20)
        {
            playerStats.stats.health += 5;
            playerStats.stats.coins -= 20;
            playerStats.GetComponentInChildren<Health>().SetHealth(playerStats.stats.health);
            UpdateUI();
        }
    }

    public void IncreaseDMG()
    {
        if (coins >= 20)
        {
            playerStats.stats.damage += 1;
            playerStats.stats.coins -= 20;
            UpdateUI();
        }
    }

    public void IncreaseSpeed()
    {
        if (coins >= 20)
        {
            playerStats.stats.speed += 1;
            playerStats.stats.coins -= 20;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        coins = playerStats.stats.coins;
        hpText.text = "Health: " + playerStats.stats.health;
        dmgText.text = "Damage: " + playerStats.stats.damage;
        speedText.text = "Speed: " + playerStats.stats.speed;
        coinsText.text = "Coins: " + coins;
    }
    public void OpenShop()
    {
        // Save previous cursor state
        previousLockState = Cursor.lockState;
        previousCursorVisibility = Cursor.visible;
        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Activate the Panel
        panel.SetActive(true);
    }

    public void CloseShop()
    {
        // Restore previous cursor state
        Cursor.lockState = previousLockState;
        Cursor.visible = previousCursorVisibility;
        // Deactivate the Panel
        panel.SetActive(false);
    }
}
