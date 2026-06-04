using System.Collections.Generic;
using UnityEngine;

public class BarbarianAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private Barbarian barbarian;

    // private string ATTACK_TRIGGER = "Attack";
    private Dictionary<int, string> AnimationIndexDictionary = new Dictionary<int, string>()
    { 
            { 0, "FirstAttack" },
            { 1, "SecondAttack" },
            { 2, "ThirdAttack" }
    };
    private int currentAnimIndex;

    private string EARTHQUAKE_TRIGGER = "Earthquake";
    private string IS_WALKING = "IsWalking";

    private void Start()
    {
        barbarian = GetComponent<Barbarian>();

        barbarian.OnCloseAttack += Barbarian_OnCloseAttack;
        barbarian.OnEarthquakeTriggered += Barbarian_OnEarthquakeTriggered;
        
        BarbarianCloseDistanceState.OnCloseAttack += Barbarian_OnCloseAttack;
        BarbarianMediumDistanceState.OnEarthquakeTriggered += Barbarian_OnEarthquakeTriggered;
    }

    private void Barbarian_OnCloseAttack()
    {
        if (currentAnimIndex >= AnimationIndexDictionary.Count)
            currentAnimIndex = 0;

        animator.SetTrigger(AnimationIndexDictionary[currentAnimIndex]);
        currentAnimIndex++;
    }

    private void Barbarian_OnEarthquakeTriggered()
    {
        animator.SetTrigger(EARTHQUAKE_TRIGGER);
    }

    private void Update()
    {
        HandleWalking();
    }

    private void HandleWalking()
    {
        animator.SetBool(IS_WALKING, barbarian.IsMoving());
    }
}