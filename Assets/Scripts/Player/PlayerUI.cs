using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerHPAmount;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image staminaBar;

    [Header("Health bar")]
    [SerializeField] private Slider frontHealthBar;
    [SerializeField] private Slider backgroundHealthBar;
    private Image frontHBImage;
    private Image backgroundHBImage;
    private Color frontHealthBarColor = new Color(189, 0, 255);
    private Color damagedHealthBarColor = Color.red;
    private Color poisonedHealthBarColor = new Color(164, 255, 0);
    private Color healingHealthBarColor = new Color(0, 255, 255);
    private float lerpSpeed = 0.05f;

    private void Start()
    {
        frontHealthBar.value = CharacterAttributes.MAX_HEALTH;
        backgroundHealthBar.value = frontHealthBar.value;

        frontHBImage = frontHealthBar.fillRect.GetComponent<Image>();
        backgroundHBImage = backgroundHealthBar.fillRect.GetComponent<Image>();
    }

    private void Update()
    {
        UpdateCharacterAttributes();
        UpdateHealthBarUI();
    }

    // TODO: Add more visuals.
    private void UpdateCharacterAttributes()
    {
        playerHPAmount.text = CharacterAttributes.Instance.Health.ToString() + " " + PlayerInput.Instance.IsBlockingPressed();

        manaBar.fillAmount = CharacterAttributes.Instance.Mana / CharacterAttributes.MAX_MANA;
        staminaBar.fillAmount = CharacterAttributes.Instance.Stamina / CharacterAttributes.MAX_STAMINA;
    }

    private void UpdateHealthBarUI()
    {
        float health = CharacterAttributes.Instance.Health;

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
