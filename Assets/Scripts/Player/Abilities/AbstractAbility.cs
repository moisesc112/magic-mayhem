using System.Collections;
using UnityEngine;

// TODO make a controller to handle adding the abilities dynamically in game
public abstract class AbstractAbility : MonoBehaviour
{
    public AbilityInfo abilityInfo;

    public virtual void Start()
    {
        // TODO Make this input button based
        StartCoroutine(UseAttackCoroutine());
    }

    public virtual IEnumerator UseAttackCoroutine()
    {
        yield return new WaitForSeconds(abilityInfo.cooldown);
        UseAttack();
        StartCoroutine(UseAttackCoroutine());
    }

    public virtual void UseAttack()
    {
        CreateProjectile();
    }

    public virtual GameObject CreateProjectile()
    {
        return Instantiate(abilityInfo.projectilePrefab, transform.position, Quaternion.identity);
    }
}
