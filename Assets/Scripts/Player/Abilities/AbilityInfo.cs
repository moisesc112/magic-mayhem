using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Info", menuName = "Ability Info")]
public class AbilityInfo : ScriptableObject
{
    public string abilityName;
    public string description;
    public AbiltyType abiltyType;
    public float cooldown;
    public float damage;
    public GameObject abilityPrefab;
    public bool projectileDestoryAfterCollision = true;
    public float despawnTime;
    public Vector3 projectileTargetDirection;
    public float projectileTargetSpeed;
}
