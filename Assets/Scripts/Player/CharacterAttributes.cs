using System;
using UnityEngine;

public class CharacterAttributes : MonoBehaviour
{
    public static CharacterAttributes Instance { get; private set; }
    public PlayerMotor playerMotorInstance;

    public event Action<Vector3> OnTakenFullHit;
    public event Action<Vector3> OnTakenBlockedHit;

    public const float MAX_HEALTH = 100F;
    public const float MAX_MANA = 100F;
    public const float MAX_STAMINA = 100F;    

    public float Health { get => health; }
    public float Mana { get => mana; }
    public float Stamina { get => stamina; }

    private float health;
    private float mana;
    private float stamina;
    private float staminaRecoveryTimer;
    private float staminaRecoveryTimerMax = 3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        health = MAX_HEALTH;
        mana = MAX_MANA;
        stamina = MAX_STAMINA;
    }

    private void Start()
    {
        Skeleton.OnPlayerHit += TakeDamage;
        Arrow.OnPlayerShot += TakeDamage;
        SkeletonMagicCharge.OnPlayerHit += TakeDamage;
        PlayerCombat.Instance.OnManaSpent+= SpendMana;
        ManaObject.OnManaRestored += RestoreMana;
        HealingObject.OnHealthRestored+= Heal;

        Barbarian.OnDamagingPlayer += TakeDamage;

        playerMotorInstance = PlayerMotor.Instance;
    }

    private void Update()
    {
        // Debug;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Heal(100f);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RestoreMana(100f);
        }

        HandleStamina();

        if (playerMotorInstance.IsFlying)
        {
            SpendMana(Time.deltaTime);
        }
    }

    public bool IsCharacterAbleToRun()
    {
        return stamina > 0;
    }

    private void TakeDamage(float damage)
    {
        if (health - damage > 0)
        {
            if (PlayerInput.Instance.IsBlockingPressed())
            {
                OnTakenBlockedHit?.Invoke(transform.position);
                health -= damage / 2;
                return;
            }

            OnTakenFullHit?.Invoke(transform.position);
            health -= damage;
        }

        else
        {
            OnTakenFullHit?.Invoke(transform.position);
            health = 0;
            // character dies.
        }
    }

    private void Heal(float hp)
    {
        if (health + hp < MAX_HEALTH)
        {
            health += hp;
        }

        else health = MAX_HEALTH;
    }

    private void SpendMana(float amount)
    {
        if (mana - amount >= 0)
        {
            mana -= amount;
        }
    }

    private void RestoreMana(float manaAmount)
    {
        if (mana + manaAmount < MAX_MANA)
        {
            mana += manaAmount;
        }

        else mana = MAX_MANA;
    }

    private void HandleStamina()
    {
        if (PlayerMotor.Instance.IsCharacterRunning())
        {
            if (IsCharacterAbleToRun())
            {
                staminaRecoveryTimer = 0f;

                stamina -= 10 * Time.deltaTime;
            }
        }

        else if (stamina < MAX_STAMINA)
        {
            staminaRecoveryTimer += Time.deltaTime;

            if (staminaRecoveryTimer >= staminaRecoveryTimerMax)
            {
                stamina += 50 * Time.deltaTime; // 20 is okay.
            }
        }

        else staminaRecoveryTimer = 0f;
    }

    private void OnDestroy()
    {
        Skeleton.OnPlayerHit -= TakeDamage;
        Arrow.OnPlayerShot -= TakeDamage;
        SkeletonMagicCharge.OnPlayerHit -= TakeDamage;
        PlayerCombat.Instance.OnManaSpent -= SpendMana;
        ManaObject.OnManaRestored -= RestoreMana;
        HealingObject.OnHealthRestored -= Heal;
        Barbarian.OnDamagingPlayer -= TakeDamage;
    }
}
