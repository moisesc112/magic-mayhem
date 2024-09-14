using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CastingComponent : MonoBehaviour
{
	void Awake()
	{
		_animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool("IsCastingSpell", _isCastingSpell);
    }

    public void UpdateCasting(bool isCastingSpell) => _isCastingSpell = isCastingSpell;

    bool _isCastingSpell;
    Animator _animator;
}
