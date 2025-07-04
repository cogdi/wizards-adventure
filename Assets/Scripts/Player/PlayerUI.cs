using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private CharacterAttributes characterAttributesInstance;

    [SerializeField] private TextMeshProUGUI playerHPAmount;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image staminaBar;

    [Header("Health bar")]
    [SerializeField] private Slider frontHealthBar;
    [SerializeField] private Slider backgroundHealthBar;
    [SerializeField] private Image frontHBImage;
    [SerializeField]private Image backgroundHBImage;
    private Color frontHealthBarColor = new Color(189, 0, 255);
    private Color damagedHealthBarColor = Color.red;
    private Color poisonedHealthBarColor = new Color(164, 255, 0);
    private Color healingHealthBarColor = new Color(0, 255, 255);
    private float lerpSpeed = 0.05f;
    private float health;

    private void Start()
    {
        characterAttributesInstance = CharacterAttributes.Instance;
        health = characterAttributesInstance.Health;

        frontHealthBar.value = CharacterAttributes.MAX_HEALTH;
        backgroundHealthBar.value = frontHealthBar.value;

        //frontHBImage = frontHealthBar.fillRect.GetComponent<Image>();
        //backgroundHBImage = backgroundHealthBar.fillRect.GetComponent<Image>();
    }

    private void Update()
    {
        health = characterAttributesInstance.Health;

        UpdateCharacterAttributes();
        UpdateHealthBarUI();
    }

    // TODO: Add more visuals to character attributes' updating.
    // Also remove string concatenations, they trigger garbage collector, use string builder in case you want to leave number HP visual.
    private void UpdateCharacterAttributes()
    {
        // Debug;
        playerHPAmount.text = characterAttributesInstance.Health.ToString() + " " + PlayerInput.Instance.IsBlockingPressed();

        manaBar.fillAmount = characterAttributesInstance.Mana / CharacterAttributes.MAX_MANA;
        staminaBar.fillAmount = characterAttributesInstance.Stamina / CharacterAttributes.MAX_STAMINA;
    }

    private void UpdateHealthBarUI()
    {
        if (health < backgroundHealthBar.value)
        {
            backgroundHBImage.color = damagedHealthBarColor;
            frontHealthBar.value = health;
            backgroundHealthBar.value = Mathf.Lerp(backgroundHealthBar.value, frontHealthBar.value, lerpSpeed);
        }

        else if (health > frontHealthBar.value)
        {
            backgroundHBImage.color = healingHealthBarColor;
            backgroundHealthBar.value = health;
            frontHealthBar.value = Mathf.Lerp(frontHealthBar.value, backgroundHealthBar.value, lerpSpeed);
        }
    }
}
