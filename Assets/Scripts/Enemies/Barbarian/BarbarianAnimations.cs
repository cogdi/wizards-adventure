using System;
using System.Collections.Generic;
using UnityEngine;

public class BarbarianAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private Barbarian barbarian;

    private Dictionary<int, string> AnimationIndexDictionary = new Dictionary<int, string>()
    { 
            { 0, "FirstAttack" },
            { 1, "SecondAttack" },
            { 2, "ThirdAttack" }
    };
    private int currentAnimIndex;

    private string EARTHQUAKE_TRIGGER = "Earthquake";
    private string IS_WALKING = "IsWalking";
    private string HEADACHE_TRIGGER = "Headache";
    private string IS_HEADACHING = "IsHeadaching";
    private string PICKUP_TRIGGER = "PickUp";
    private string SPIN_TRIGGER = "Spin";


    private void Start()
    {
        barbarian = GetComponent<Barbarian>();

        barbarian.OnCloseAttack += Barbarian_OnCloseAttack;
        barbarian.OnEarthquakeTriggered += Barbarian_OnEarthquakeTriggered;
        barbarian.OnWallHitInRush += Barbarian_OnWallHitInRush;
        barbarian.OnStoneCrushed += Barbarian_OnStoneCrushed;

        BarbarianCloseDistanceState.OnCloseAttack += Barbarian_OnCloseAttack;
        BarbarianMediumDistanceState.OnEarthquakeTriggered += Barbarian_OnEarthquakeTriggered;
        BarbarianLongDistanceState.OnHeadachePassed += Barbarian_OnHeadachePassed;
        BarbarianLongDistanceState.OnPickingUpStone += BarbarianLongDistanceState_OnPickingUpStone;
    }

    private void Barbarian_OnStoneCrushed()
    {
        animator.SetBool(SPIN_TRIGGER, true);
    }

    private void BarbarianLongDistanceState_OnPickingUpStone()
    {
        animator.SetTrigger(PICKUP_TRIGGER);
    }


    private void Barbarian_OnHeadachePassed()
    {
        animator.SetBool(IS_HEADACHING, false);
    }

    private void Barbarian_OnWallHitInRush()
    {
        animator.SetTrigger(HEADACHE_TRIGGER);
        animator.SetBool(IS_HEADACHING, true);
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

    private void OnDestroy()
    {
        BarbarianCloseDistanceState.OnCloseAttack -= Barbarian_OnCloseAttack;
        BarbarianMediumDistanceState.OnEarthquakeTriggered -= Barbarian_OnEarthquakeTriggered;
        BarbarianLongDistanceState.OnHeadachePassed -= Barbarian_OnHeadachePassed;
        BarbarianLongDistanceState.OnPickingUpStone -= BarbarianLongDistanceState_OnPickingUpStone;
    }
}