using UnityEngine;

public class AbilitySlotsComponent : MonoBehaviour
{
    public AbilityInfo abilitySlot1;
    public AbilityInfo abilitySlot2;
    public AbilityInfo abilitySlot3;
    
    public float ability1Cooldown = 0;
    public float ability2Cooldown = 0;
    public float ability3Cooldown = 0;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _selectedAbililtyNumber = 1;
    }

    private void Update()
    {
        if (ability1Cooldown > 0) SetAbilityCooldown(1, ability1Cooldown -= Time.deltaTime);
        if (ability2Cooldown > 0) SetAbilityCooldown(2, ability2Cooldown -= Time.deltaTime);
        if (ability3Cooldown > 0) SetAbilityCooldown(3, ability3Cooldown -= Time.deltaTime);
    }

    public bool CanCast() => GetAbilityCooldown(_selectedAbililtyNumber) <= 0;

    public void UpdateAbilitySlot(AbilityInfo newAbility, int slotNumber)
    {
        switch (slotNumber)
        {
            case 1:
                abilitySlot1 = newAbility;
                break;
            case 2:
                abilitySlot2 = newAbility;
                break;
            case 3:
                abilitySlot3 = newAbility;
                break;
        }
    }

    public AbilityInfo GetAbility(int slotNumber)
    {
        return slotNumber switch
        {
            1 => abilitySlot1,
            2 => abilitySlot2,
            3 => abilitySlot3,
            _ => throw new System.NotImplementedException(),
        };
    }

    public float GetAbilityCooldown(int slotNumber)
    {
        return slotNumber switch
        {
            1 => ability1Cooldown,
            2 => ability2Cooldown,
            3 => ability3Cooldown,
            _ => throw new System.NotImplementedException(),
        };
    }

    public void SetAbilityCooldown(int slotNumber, float newValue)
    {
        switch (slotNumber)
        {
            case 1:
                ability1Cooldown = newValue;
                break;
            case 2:
                ability2Cooldown = newValue;
                break;
            case 3:
                ability3Cooldown = newValue;
                break;
        }
    }

    public void CastSpell()
    {
        var ability = GetAbility(_selectedAbililtyNumber);

		SetAbilityCooldown(_selectedAbililtyNumber, ability.cooldown);
		var abilityPrefab = Instantiate(ability.abilityPrefab, _player.GetAvatarPosition(), Quaternion.identity);
		var abilityComponent = abilityPrefab.GetComponent<Ability>();
		if (abilityComponent != null)
		{
			abilityComponent.SetPlayer(_player);
		}
	}

    public void SetSelectedAbility(int slotNumber)
    {
        var ability = GetAbility(slotNumber);
        if (ability != null)
        {
			_selectedAbililtyNumber = slotNumber;

        }
    }

    private Player _player;
    int _selectedAbililtyNumber;
}
