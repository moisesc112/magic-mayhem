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
        if (_isCastingSpell && _abilitySlotsComponent.CanCast())
        {
            _animator.SetBool("IsCastingSpell", true);
            _abilitySlotsComponent.CastSpell();
        }
        else
        {
			_animator.SetBool("IsCastingSpell", false);
        }
	}

    public void SetSelectedAbility(int slotNumber)
    {
        _abilitySlotsComponent.SetSelectedAbility(slotNumber);
    }

    public void UpdateCasting(bool isCastingSpell) => _isCastingSpell = isCastingSpell;

    bool _isCastingSpell;
    Animator _animator;
    AbilitySlotsComponent _abilitySlotsComponent;
}
