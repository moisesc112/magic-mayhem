using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Info", menuName = "Trap Info")]
public class TrapInfo : ScriptableObject
{
    //Not sure if its better to have isActive in the AbstractTrap file itself, since its getting auto-disabled when I run the game
    //TODO make isActive depedent on player turning it on

    //public bool isActive;
    public float cooldown;
    public float damage;
}
