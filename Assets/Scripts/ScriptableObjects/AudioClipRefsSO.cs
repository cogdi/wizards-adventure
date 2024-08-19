using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
    public AudioClip hitWall;
    public AudioClip[] hitSkeleton;
    public AudioClip hitWallByCharge;
    public AudioClip hitSkeletonByCharge;
    public AudioClip[] playerFullDamage;
    public AudioClip[] playerBlockedDamage;
}