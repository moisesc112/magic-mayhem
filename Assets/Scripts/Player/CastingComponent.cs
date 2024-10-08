using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AbilitySlotsComponent), typeof(Mover))]
public class CastingComponent : MonoBehaviour
{
    [SerializeField] Transform _castingLocation;
	void Awake()
	{
		_animator = GetComponent<Animator>();
        _abilitySlotsComponent = GetComponent<AbilitySlotsComponent>();
        _mover = GetComponent<Mover>();
	}

    // Update is called once per frame
    void Update()
    {
        if (!_mover.isRolling && _isCastingSpell && _abilitySlotsComponent.CanCast())
        {
            _castRight = !_castRight;
            _animator.SetInteger("CastVariant", Random.Range(1, 4));
			_animator.SetTrigger($"{(_castRight ? "R" : "L")}Cast");
            _abilitySlotsComponent.CastSpell();
        }
	}

    public Vector3 GetCastingPosition() => _castingLocation.position;
    public Quaternion GetRandomCastingSpreadRotation(float angle)
    {
        var randomY = Random.Range(-angle, angle);

        return Quaternion.LookRotation(_castingLocation.forward) * Quaternion.Euler(0, randomY, 0);
    }

    public void SetSelectedAbility(int slotNumber)
    {
        _abilitySlotsComponent.SetSelectedAbility(slotNumber);
    }

    public void UpdateCasting(bool isCastingSpell) => _isCastingSpell = isCastingSpell;

    bool _isCastingSpell;
    bool _castRight;
    Animator _animator;
    AbilitySlotsComponent _abilitySlotsComponent;
    Mover _mover;
}
