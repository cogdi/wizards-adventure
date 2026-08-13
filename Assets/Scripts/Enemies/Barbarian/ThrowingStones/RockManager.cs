using System;
using UnityEngine;

public class RockManager : MonoBehaviour
{
    public static RockManager Instance { get; private set; }

    public event Action<GameObject> OnRockFallen;

    private const string BARBARIAN_ROCK_PATH = "Prefabs/Barbarian_Rock";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;        
    }

    public void TriggerRockFalling()
    {
        GameObject go = Instantiate(Resources.Load(BARBARIAN_ROCK_PATH) as GameObject, 
                        position: transform.position  + UnityEngine.Random.insideUnitSphere * 5f, 
                        Quaternion.identity
        );

        OnRockFallen?.Invoke(go);
    }
}