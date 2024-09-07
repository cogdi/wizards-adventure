using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffSounds : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chargingSound;
    [SerializeField] private AudioClip firingSound;

    private void Awake()
    {
        audioSource.volume = 0.3f;
    }

    private void Start()
    {
        PlayerCombat.Instance.OnStaffStateChanged += PlayerCombat_OnStaffStateChanged;
    }

    private void PlayerCombat_OnStaffStateChanged(PlayerCombat.StaffState staffState)
    {
        audioSource.Pause();

        if (staffState == PlayerCombat.StaffState.Charging)
        {
            audioSource.clip = chargingSound;
            audioSource.Play();
        }

        else
        {
            audioSource.clip = firingSound;
            audioSource.Play();
        }
    }
}
