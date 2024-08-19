using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public event Action<Vector3> OnAnySoundMade;

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerCombat.Instance.OnWallHit += PlayerCombat_OnWallHit;
        PlayerCombat.Instance.OnSkeletonHit += PlayerCombat_OnSkeletonHit;
        MagicCharge.OnWallHit += MagicCharge_OnWallHit;
        MagicCharge.OnSkeletonHit += MagicCharge_OnSkeletonHit;
        CharacterAttributes.Instance.OnTakenFullHit += CharacterAttributes_OnTakenFullHit;
        CharacterAttributes.Instance.OnTakenBlockedHit += CharacterAttributes_OnTakenBlockedHit;
    }

    private void MagicCharge_OnSkeletonHit(object sender, MagicCharge.OnAnyHitEventArgs e)
    {
        PlaySound(audioClipRefsSO.hitSkeletonByCharge, e.hitPosition, 1f);
    }

    private void MagicCharge_OnWallHit(object sender, MagicCharge.OnAnyHitEventArgs e)
    {
        PlaySound(audioClipRefsSO.hitWallByCharge, e.hitPosition, 1f);
    }

    private void CharacterAttributes_OnTakenBlockedHit(Vector3 hitPosition)
    {
        PlaySoundArray(audioClipRefsSO.playerBlockedDamage, hitPosition, 1f);
    }

    private void CharacterAttributes_OnTakenFullHit(Vector3 hitPosition)
    {
        PlaySoundArray(audioClipRefsSO.playerFullDamage, hitPosition, 1f);
    }

    private void PlayerCombat_OnSkeletonHit(Vector3 hitPosition)
    {
        PlaySoundArray(audioClipRefsSO.hitSkeleton, hitPosition, 1f);
    }

    private void PlayerCombat_OnWallHit(Vector3 hitPosition)
    {
        PlaySound(audioClipRefsSO.hitWall, hitPosition, 1f);
    }

    private void PlaySoundArray(AudioClip[] audioClipArray, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume);
        OnAnySoundMade?.Invoke(position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
        OnAnySoundMade?.Invoke(position);
    }

    private void OnDestroy()
    {
        MagicCharge.OnWallHit -= MagicCharge_OnWallHit;
        MagicCharge.OnSkeletonHit -= MagicCharge_OnSkeletonHit;
    }
}
