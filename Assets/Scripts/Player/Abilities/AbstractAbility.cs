using System.Collections;
using UnityEngine;

// TODO make a controller to handle adding the abilities dynamically in game
public abstract class AbstractAbility : MonoBehaviour
{
    public AbilityInfo AbilityInfo;

    public virtual void Start()
    {
        // TODO Make this input button based
        StartCoroutine(UseAttackCoroutine());
    }

    public virtual IEnumerator UseAttackCoroutine()
    {
        yield return new WaitForSeconds(AbilityInfo.cooldown);
        UseAttack();
        StartCoroutine(UseAttackCoroutine());
    }

    public virtual void UseAttack()
    {
        CreateProjectile();
    }

    public virtual GameObject CreateProjectile(string prefabPath = "")
    {
        var projectileGameObject = Instantiate(AbilityInfo.projectilePrefab, transform.position, Quaternion.identity);
        projectileGameObject.GetComponent<AbstractProjectile>().IncomingDamage = AbilityInfo.damage;
        return projectileGameObject;
    }
}
