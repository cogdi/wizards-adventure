using UnityEngine;

public class Key : MonoBehaviour
{
    public int keyID { get => _keyID; }

    [SerializeField] private int _keyID;
}