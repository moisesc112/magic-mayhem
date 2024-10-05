using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Info", menuName = "Ability Info")]
public class AbilityInfo : ScriptableObject
{
    public string abilityName;
    public string description;
    public AbiltyType abiltyType;
    public float cooldown;
    public float damage;
    public int cost;
    public GameObject abilityPrefab;
    public bool projectileDestoryAfterCollision = true;
    public float despawnTime;
    public Vector3 projectileTargetDirection;
    public float projectileTargetSpeed;
    public float projectileSpread = 0.0f;
    public AudioClip castingSound;
}
