using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Info", menuName = "Ability Info")]
public class AbilityInfo : ScriptableObject
{
    public string abilityName;
    public string description;
    public float cooldown;
    public float damage;
    public GameObject projectilePrefab;
}
