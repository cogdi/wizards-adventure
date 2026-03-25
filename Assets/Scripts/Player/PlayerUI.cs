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

    private bool isStaminaChanging;
    
    private void Start()
    {
        characterAttributesInstance = CharacterAttributes.Instance;
        
        characterAttributesInstance.OnHealthChanged += CharacterAttributes_OnHealthChanged;
        characterAttributesInstance.OnManaChanged += CharacterAttributes_OnManaChanged;
        characterAttributesInstance.OnStaminaStateChanged += CharacterAttributes_OnStaminaChanged;
        
    }
    
    private void Update()
    {
        if (isStaminaChanging)
        {
            staminaSlider.value = characterAttributesInstance.Stamina;
        }
    }
    
    private void CharacterAttributes_OnHealthChanged()
    {
        hpSlider.value = characterAttributesInstance.Health;
    }
    
    private void CharacterAttributes_OnManaChanged()
    {
        manaSlider.value = characterAttributesInstance.Mana;                
    }
    
    private void CharacterAttributes_OnStaminaChanged(bool isChanging)
    {
        isStaminaChanging = isChanging;
    }
}
