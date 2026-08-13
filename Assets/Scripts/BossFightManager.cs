using System;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    public static BossFightManager Instance { get; private set; }

    public enum Boss
    {
        Barbarian,
        Witch
    }

    public static event Action OnBossFightTriggered;
    public static bool IsBossFightTriggered;
    public event Action OnBossFightEnded;
    //[SerializeField] private Boss fightBoss;
    private EnemyBase currentBoss; // NOTE: Note this field if Witch won't be a part of EnemyBase in the future.

    private const string BARBARIAN_PREFAB_PATH = "Prefabs/Barbarian";
    private const string WITCH_PREFAB_PATH = "";
    [SerializeField] private Transform bossSpawnPosition;
    [SerializeField] private Transform[] BossFightTriggers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < BossFightTriggers.Length; i++)
            BossFightTriggers[i].GetComponent<BossFightTrigger>().OnBossFightTriggered += SummonBoss;

        Barbarian.OnBarbarianBeated += Barbarian_OnBarbarianBeated;
    }

    private void Barbarian_OnBarbarianBeated()
    {
        OnBossFightEnded?.Invoke();   
    }

    // private void BossFightTrigger_OnBossFightTriggered()
    // {
    //     SummonBoss(fightBoss);
    // }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (!IsBossFightTriggered)
    //     {
    //         if (other.CompareTag(PlayerCombat.PLAYER_TAG))
    //         {
    //             IsBossFightTriggered = true;
    //             OnBossFightTriggered?.Invoke();
    //             Debug.Log("Collided with " + other.name);

    //             SummonBoss(fightBoss);
    //         }
    //     }
    // }



    private void SummonBoss(Boss boss)
    {
        //if (fightBoss)
        switch (boss)
        {
            case Boss.Barbarian:
                currentBoss = Instantiate(Resources.Load(BARBARIAN_PREFAB_PATH) as GameObject, position: bossSpawnPosition.position, Quaternion.identity)
                    .GetComponent<Barbarian>();
                break;
            case Boss.Witch:
                throw new NotImplementedException();
        }

        OnBossFightTriggered?.Invoke();
    }

    public float GetCurrentBossHP()
    {
        return currentBoss.Health;
    }
}
