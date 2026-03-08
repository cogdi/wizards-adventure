using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private CharacterAttributes characterAttributesInstance;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private Slider staminaSlider;

    private float health;
    private float mana;
    private float stamina;


    private void Start()
    {
        characterAttributesInstance = CharacterAttributes.Instance;
    }

    private void Update()
    {
        health = characterAttributesInstance.Health;
        mana = characterAttributesInstance.Mana;
        stamina = characterAttributesInstance.Stamina;
    }

    private void LateUpdate()
    {
        hpText.text = health.ToString();

        hpSlider.value = health;
        manaSlider.value = mana;
        staminaSlider.value = stamina;
    }


    // private CharacterAttributes characterAttributesInstance;

    // [SerializeField] private TextMeshProUGUI playerHPAmount;
    // [SerializeField] private Image manaBar;
    // [SerializeField] private Image staminaBar;

    // [Header("Health bar")]
    // [SerializeField] private Slider frontHealthBar;
    // [SerializeField] private Slider backgroundHealthBar;
    // [SerializeField] private Image frontHBImage;
    // [SerializeField]private Image backgroundHBImage;
    // private Color frontHealthBarColor = new Color(189, 0, 255);
    // private Color damagedHealthBarColor = Color.red;
    // private Color poisonedHealthBarColor = new Color(164, 255, 0);
    // private Color healingHealthBarColor = new Color(0, 255, 255);
    // private float lerpSpeed = 0.05f;
    // private float health;
    
    // private float mana;
    // private float stamina;

    // private void Start()
    // {
    //     characterAttributesInstance = CharacterAttributes.Instance;
    //     health = characterAttributesInstance.Health;

    //     mana = CharacterAttributes.MAX_MANA;
    //     stamina = CharacterAttributes.MAX_STAMINA;

    //     frontHealthBar.value = CharacterAttributes.MAX_HEALTH;
    //     backgroundHealthBar.value = frontHealthBar.value;
    // }

    // private void Update()
    // {
    //     health = characterAttributesInstance.Health;
        
    //     mana = characterAttributesInstance.Mana;
    //     stamina = characterAttributesInstance.Stamina;

    //     UpdateCharacterAttributes();
    //     UpdateHealthBarUI();
    // }

    // // TODO: Add more visuals to character attributes' updating.
    // // Also remove string concatenations, they trigger garbage collector, use string builder in case you want to leave number HP visual.
    // private void UpdateCharacterAttributes()
    // {
    //     // Debug;
    //     playerHPAmount.text = characterAttributesInstance.Health.ToString() + " " + PlayerInput.Instance.IsBlockingPressed();

    //     //manaBar.fillAmount = mana / CharacterAttributes.MAX_MANA;
    //     //staminaBar.fillAmount = stamina / CharacterAttributes.MAX_STAMINA;

    //     if (manaBar.fillAmount != mana)
    //     {
    //         manaBar.fillAmount = Mathf.Lerp(manaBar.fillAmount, mana / 100, lerpSpeed);
    //     }

    //     if (staminaBar.fillAmount != stamina)
    //     {
    //         staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, stamina / 100, lerpSpeed);
    //     }
    // }

    // private void UpdateHealthBarUI()
    // {
    //     if (health < backgroundHealthBar.value)
    //     {
    //         backgroundHBImage.color = damagedHealthBarColor;
    //         frontHealthBar.value = health;
    //         backgroundHealthBar.value = Mathf.Lerp(backgroundHealthBar.value, frontHealthBar.value, lerpSpeed);
    //     }

    //     else if (health > frontHealthBar.value)
    //     {
    //         backgroundHBImage.color = healingHealthBarColor;
    //         backgroundHealthBar.value = health;
    //         frontHealthBar.value = Mathf.Lerp(frontHealthBar.value, backgroundHealthBar.value, lerpSpeed);
    //     }
    // }
}
