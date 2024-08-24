using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public abstract class Enemy : MonoBehaviour
{
    /* 
     * This class is created to be used by all the types of enemies in the game. 
     * It contains all the mutual logic between enemy classes.
    */

    public float Health => health;
    public NavMeshAgent Agent { get => agent; }
    protected const float MAX_HEALTH = 100f;

    protected float health = 0f;
    protected NavMeshAgent agent;
    protected Transform playerTransform;
    protected Vector3 playerLastPosition;
    protected PlayerCombat playerCombatInstance;

    // Raycast
    protected float eyeLevel = 1.15f;
    protected float sightDistance = 15f;
    protected float fieldOfView = 100f;
    protected int ignoreRaycastMask;
    //[SerializeField] protected LayerMask playerLayer;
    protected LayerMask playerLayer;

    protected virtual void Awake()
    {
        health = MAX_HEALTH;
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0.1f;
    }

    protected virtual void Start()
    {
        playerCombatInstance = PlayerCombat.Instance;
        //playerTransform = GameObject.FindGameObjectWithTag(PlayerCombat.PLAYER_TAG).transform;
        playerTransform = playerCombatInstance.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player instance can not be found.");
        }

        ignoreRaycastMask = ~LayerMask.GetMask("IgnoreSkeletonRaycast");

        playerLayer = LayerMask.GetMask(PlayerCombat.PLAYER_TAG);
    }

    public virtual void TakeDamage(Enemy enemy, float damage) // TODO: Maybe I should make it abstract, or neither virtual or abstract. MAKE IT PROTECTED.
    {
        if (enemy == this)
        {
            // To get the Skeleton know about the player's position.
            // TODO: Make a proper system with audio. If any sound plays in some range (10f), skeletons will hear it and come to it.

            health -= damage;
            Debug.Log(health);

            if (health <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public bool CanSeePlayer()
    {
        if (GetDistanceToPlayer() <= sightDistance)
        {
            Vector3 playerDirection = playerTransform.position - transform.position;
            if (Vector3.Angle(playerDirection, transform.forward) <= fieldOfView)
            {
                if (Physics.Raycast(transform.position + (Vector3.up * eyeLevel), playerDirection, out RaycastHit hitInfo, sightDistance, ignoreRaycastMask))
                {
                    if (playerCombatInstance.IsPlayerLayer(hitInfo.transform.gameObject.layer))
                    {
                        playerLastPosition = playerTransform.position;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    //protected bool IsPlayerLayer(int layer)
    //{
    //    return playerLayer == (playerLayer | (1 << layer));
    //}

    public Vector3 GetPlayerLastPosition()
    {
        return playerLastPosition;
    }

    public bool IsMoving()
    {
        return agent.velocity.magnitude > 0.1f;
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(playerTransform.position, transform.position);
    }
}

