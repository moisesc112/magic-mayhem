using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AbilitySlotsComponent))]
public class CastingComponent : MonoBehaviour
{
    [SerializeField] Transform _castingLocation;
	void Awake()
	{
		_animator = GetComponent<Animator>();
        _abilitySlotsComponent = GetComponent<AbilitySlotsComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool("IsCastingSpell", _isCastingSpell);
    }

    public void UseAbility(int slotNumber)
    {
        _abilitySlotsComponent.UseAbility(slotNumber);
    }

    public void UpdateCasting(bool isCastingSpell) => _isCastingSpell = isCastingSpell;

    bool _isCastingSpell;
    Animator _animator;
    AbilitySlotsComponent _abilitySlotsComponent;
}
