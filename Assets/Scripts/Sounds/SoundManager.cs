using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    public event Action<Vector3> OnAnySoundMade;

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PlayerCombat.Instance.OnWallHit += PlayerCombat_OnWallHit;
        PlayerCombat.Instance.OnEnemyHit += PlayerCombat_OnEnemyHit;
        MagicCharge.OnWallHit += MagicCharge_OnWallHit;
        MagicCharge.OnEnemyHit += MagicCharge_OnEnemyHit;
        CharacterAttributes.Instance.OnTakenFullHit += CharacterAttributes_OnTakenFullHit;
        CharacterAttributes.Instance.OnTakenBlockedHit += CharacterAttributes_OnTakenBlockedHit;
    }

    private void MagicCharge_OnEnemyHit(Vector3 hitPosition)
    {
        PlaySound(audioClipRefsSO.hitSkeletonByCharge, hitPosition, 1f);
    }

    private void MagicCharge_OnWallHit(Vector3 hitPosition)
    {
        PlaySound(audioClipRefsSO.hitWallByCharge, hitPosition, 1f);
    }

    private void CharacterAttributes_OnTakenBlockedHit(Vector3 hitPosition)
    {
        PlaySoundArray(audioClipRefsSO.playerBlockedDamage, hitPosition, 1f);
    }

    private void CharacterAttributes_OnTakenFullHit(Vector3 hitPosition)
    {
        PlaySoundArray(audioClipRefsSO.playerFullDamage, hitPosition, 1f);
    }

    private void PlayerCombat_OnEnemyHit(Vector3 hitPosition)
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
        PlayerCombat.Instance.OnWallHit -= PlayerCombat_OnWallHit;
        PlayerCombat.Instance.OnEnemyHit -= PlayerCombat_OnEnemyHit;
        MagicCharge.OnWallHit -= MagicCharge_OnWallHit;
        MagicCharge.OnEnemyHit -= MagicCharge_OnEnemyHit;
        CharacterAttributes.Instance.OnTakenFullHit -= CharacterAttributes_OnTakenFullHit;
        CharacterAttributes.Instance.OnTakenBlockedHit -= CharacterAttributes_OnTakenBlockedHit;
    }
}
