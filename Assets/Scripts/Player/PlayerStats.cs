using System.Linq;
using UnityEngine;

[RequireComponent(typeof(StatusEffects))]
public class PlayerStats : HealthComponent
{
    private float damage;
    private float healthRegenPerSecond;
    private float armor;
    private float dodgeChance;
    public int gold;

    public float GetAbilityDamage(float abilityDamage = 0) => GetStatWithStatusEffects(damage + abilityDamage, StatusEffectStat.Damage);
    public float HealthRegenPerSecond => GetStatWithStatusEffects(healthRegenPerSecond, StatusEffectStat.HealthRegenPerSecond);
    public float Armor => GetStatWithStatusEffects(armor, StatusEffectStat.Armor);
    public float DodgeChance => GetStatWithStatusEffects(dodgeChance, StatusEffectStat.DodgeChance);
    public bool IsShielded => statusEffects.currentStatusEffects.Any(x => x.stat == StatusEffectStat.Shield);
    public bool IsInvulnerable => statusEffects.currentStatusEffects.Any(x => x.stat == StatusEffectStat.Invulnerable);
    public override void Awake()
    {
        statusEffects = GetComponent<StatusEffects>();
        base.Awake();
    }

    private void Update()
    {
        health += HealthRegenPerSecond * Time.deltaTime;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public override void TakeDamage(float damage) 
    {
        if (IsShielded)
        {
            Debug.Log("Attack Shielded");
            return;
        }
        var randomDodgeChance = Random.Range(0f, 100f);
        if (randomDodgeChance < DodgeChance)
        {
            Debug.Log("Attack Dodged");
            return;
        }
        var armorReducedDamage = damage * GetPercentDamageTakenWithArmor(Armor);
        if (IsInvulnerable)
        {
            Debug.Log("Player is Invulnerable");
            armorReducedDamage = 0;
        }
        base.TakeDamage(armorReducedDamage);
    }

    public float GetPercentDamageTakenWithArmor(float armor)
    {
        return 1 / (1 + (armor / 15));
    }

    public float GetStatWithStatusEffects(float baseValue, StatusEffectStat statusEffectStat)
    {
        var currentStatusEffectsForStat = statusEffects.currentStatusEffects.Where(x => x.stat == statusEffectStat);
        var flatIncreasedStatValue = currentStatusEffectsForStat.Where(x => x.valueType == StatusEffectValueType.Flat).Sum(x => x.value);
        var perctangeIncreasedStatValue = 1 + currentStatusEffectsForStat.Where(x => x.valueType == StatusEffectValueType.Percentage).Sum(x => x.value);
        var statValue = (baseValue + flatIncreasedStatValue) * perctangeIncreasedStatValue;
        return Mathf.Max(statValue, 0);
    }

    public StatusEffects StatusEffects => statusEffects;
    [SerializeField] StatusEffects statusEffects;
}
