using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance { get; private set; }

    public event Action<float> OnManaSpent;
    public event Action<Vector3> OnWallHit;
    public event Action<Vector3> OnEnemyHit; /* TODO: It looks like a bad practice to have separate events for sound and for making damage. */
    public event Action<Enemy, float> OnEnemyDamaged;

    public event Action<bool> OnChargingMagicAttack;
    public event Action<StaffState> OnStaffStateChanged;

    public const string PLAYER_TAG = "Player";
    public const string LIGHT_MAGIC_CHARGE_TAG = "WizardLightMagicCharge";
    public const string MEDIUM_MAGIC_CHARGE_TAG = "WizardMediumMagicCharge";
    public const string STRONG_MAGIC_CHARGE_TAG = "WizardStrongMagicCharge";

    private int playerLayer;

    public enum MagicAttacks
    {
        Light,
        Medium,
        Strong
    }

    public enum StaffState
    {
        Charging,
        Fired,
    }

    public readonly Dictionary<string, int> MagicAttacksDamageDictionary = new()
    {
        { LIGHT_MAGIC_CHARGE_TAG, 20 },
        { MEDIUM_MAGIC_CHARGE_TAG, 35 },
        { STRONG_MAGIC_CHARGE_TAG, 100 } // Debug value. To be returned to 50.
    };

    private PlayerInput playerInputInstance;

    // Damaging enemies.
    // Melee attacks.
    private Collider[] hitColliders = new Collider[10];
    private readonly float meleeDamage = 34f;
    [SerializeField] private Transform attackRangeSphere;
    [SerializeField] private float attackRangeSphereRadius = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    // Magic attacks.
    private MagicAttacks currentAttackType; // To be used in ShootMagicCharge() that called by animation event.
    private float magicAttackIntervalTimer;
    private float magicAttackIntervalTimerMax = 1.25f;
    private float magicChargePowerTimer;
    private readonly int[] manaCostsArray = new int[3] { 10, 15, 30 };
    private StaffState currentStaffState;
    [SerializeField] private Transform magicChargePosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        playerLayer = gameObject.layer;
    }

    private void Start()
    {
        playerInputInstance = PlayerInput.Instance;
    }

    private void Update()
    {
        if (!playerInputInstance.IsBlockingPressed())
        {
            HandleMagicAttacks();
        }
    }

    // Animation event within Player object's "1H_Melee_Attack_Slice_Diagonal" calls it.
    public void AttackEnemy() // TODO: rename? EXPERIMENTAL METHOD (Try to play the game and hit more than 100 colliders at all).
    {
        int numHitColliders = Physics.OverlapSphereNonAlloc(attackRangeSphere.position, attackRangeSphereRadius, hitColliders);

        for (int i = 0; i < numHitColliders; i++)
        {
            Collider collider = hitColliders[i];
            if (IsEnemyLayer(collider.gameObject.layer))
            {
                collider.TryGetComponent<Enemy>(out Enemy enemy);
                if (enemy != null)
                {
                    OnEnemyHit?.Invoke(collider.ClosestPointOnBounds(attackRangeSphere.position));
                    OnEnemyDamaged?.Invoke(enemy, meleeDamage);
                }
            }

            else
            {
                if (collider != null)
                {
                    OnWallHit?.Invoke(collider.ClosestPointOnBounds(attackRangeSphere.position));
                }
            }
        }
    }

    public bool IsPlayerLayer(int layer)
    {
        return playerLayer == layer;
    }

    public bool IsEnemyLayer(int layer)
    {
        return enemyLayer == (enemyLayer | 1 << layer);
    }

    private void HandleMagicAttacks()
    {
        if (magicAttackIntervalTimer < magicAttackIntervalTimerMax)
            magicAttackIntervalTimer += Time.deltaTime;

        else if (!PlayerMotor.Instance.IsCharacterRunning())
        //else if (5 == 5)
        {
            if (playerInputInstance.IsMagicAttackTriggered())
            {
                OnStaffStateChanged?.Invoke(StaffState.Charging);
                magicChargePowerTimer += Time.deltaTime;
                OnChargingMagicAttack?.Invoke(true);
            }

            else if (magicChargePowerTimer > 0f)
            {
                OnStaffStateChanged?.Invoke(StaffState.Fired);
                OnChargingMagicAttack?.Invoke(false);

                // The button's been just released.
                if (magicChargePowerTimer < 2f)
                {
                    currentAttackType = MagicAttacks.Light;
                    //ShootMagicCharge(MagicAttacks.Light);
                }

                else if (magicChargePowerTimer < 3f)
                {
                    currentAttackType = MagicAttacks.Medium;
                    //ShootMagicCharge(MagicAttacks.Medium);
                }

                else
                {
                    currentAttackType = MagicAttacks.Strong;
                    //ShootMagicCharge(MagicAttacks.Strong);
                }

                magicAttackIntervalTimer = 0f;
                magicChargePowerTimer = 0f;
            }
        }
    }


    public void ShootMagicCharge()
    {
        GameObject magicCharge = null;

        if (CharacterAttributes.Instance.Mana < manaCostsArray[(int)currentAttackType])
        {
            Debug.Log("Can't proceed attack! Not enough mana.");
            return;
        }

        else
        {
            OnManaSpent?.Invoke(manaCostsArray[(int)currentAttackType]);
        }

        switch (currentAttackType)
        {
            case MagicAttacks.Light:
                magicCharge = Instantiate(Resources.Load($"Prefabs/{LIGHT_MAGIC_CHARGE_TAG}") as GameObject, magicChargePosition.position, transform.rotation);
                break;
            case MagicAttacks.Medium:
                magicCharge = Instantiate(Resources.Load($"Prefabs/{MEDIUM_MAGIC_CHARGE_TAG}") as GameObject, magicChargePosition.position, transform.rotation);
                break;
            case MagicAttacks.Strong:
                magicCharge = Instantiate(Resources.Load($"Prefabs/{STRONG_MAGIC_CHARGE_TAG}") as GameObject, magicChargePosition.position, transform.rotation);
                break;
        }

        Vector3 shootDirection = PlayerLook.Instance.GetCameraTransformForward().normalized;
        magicCharge.GetComponent<Rigidbody>().velocity = shootDirection * 17.5f;
        if (Vector3.Distance(magicChargePosition.position, magicCharge.transform.position) > 5f)
        {
            Destroy(magicCharge);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackRangeSphere != null)
        {
            Gizmos.DrawWireSphere(attackRangeSphere.position, attackRangeSphereRadius);
        }
    }
}
