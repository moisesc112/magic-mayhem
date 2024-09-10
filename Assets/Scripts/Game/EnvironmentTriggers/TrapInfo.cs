using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Info", menuName = "Trap Info")]
public class TrapInfo : ScriptableObject
{
    public bool isActive;
    public float cooldown;
    public float damage;
}
