using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public int[] skinPrices = {100, 200, 300};
    public Material[] skinMaterials;
    public Projectile projectilePrefab;

    [Header("UI Elements")]
    public GameObject shopPanel;
    public Text scoreText;
    public Button[] skinButtons;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ScoreManager.LoadScore();
        UpdateShopUI();
        CloseShop(); // Ensure the shop is closed at the start

        // Load previously selected skin, default to no skin (-1)
        int selectedSkin = PlayerPrefs.GetInt("SelectedSkin", -1);
        ApplySkin(selectedSkin);
    }

    public void BuySkin(int skinIndex)
    {
        if (ScoreManager.playerScore >= skinPrices[skinIndex])
        {
            ScoreManager.playerScore -= skinPrices[skinIndex];
            ApplySkin(skinIndex);
            PlayerPrefs.SetInt("SelectedSkin", skinIndex);
            PlayerPrefs.Save();
            UpdateShopUI();
        }
        else
        {
            Debug.Log("Not enough points to buy this skin!");
        }
    }

    void ApplySkin(int skinIndex)
    {
        if (skinIndex >= 0 && skinIndex < skinMaterials.Length)
        {
            if (projectilePrefab != null)
            {
                projectilePrefab.ApplySkin(skinIndex);
                Debug.Log($"Skin {skinIndex} applied through shop.");
            }
        }
        else
        {
            Debug.LogWarning($"Attempted to apply invalid skin index {skinIndex}. Defaulting to no skin.");
            projectilePrefab.ApplyDefaultSkin();
        }
    }

    public void UpdateShopUI()
    {
        scoreText.text = "Points: " + ScoreManager.playerScore;
        for (int i = 0; i < skinButtons.Length; i++)
        {
            skinButtons[i].interactable = ScoreManager.playerScore >= skinPrices[i];
        }
    }

    public void OpenShop()
    {
        UpdateShopUI(); // Ensure the score is updated when the shop is opened
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }
}
