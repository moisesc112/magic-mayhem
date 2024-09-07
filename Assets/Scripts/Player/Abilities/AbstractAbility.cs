using System.Collections;
using UnityEngine;

// TODO make a controller to handle adding the abilities dynamically in game
public abstract class AbstractAbility : MonoBehaviour
{
    public abstract string AbilityName { get; set; }
    public abstract string Description { get; set; }
    public abstract float Cooldown { get; set; }
    public abstract float Damage { get; set; }
    public abstract string ProjectilePrefabPath { get; set; }

    public virtual void Start()
    {
        // TODO Make this input button based
        StartCoroutine(UseAttackCoroutine());
    }

    public virtual IEnumerator UseAttackCoroutine()
    {
        yield return new WaitForSeconds(Cooldown);
        UseAttack();
        StartCoroutine(UseAttackCoroutine());
    }

    public virtual void UseAttack()
    {
        CreateProjectile();
    }

    public virtual GameObject CreateProjectile(string prefabPath = "")
    {
        prefabPath = prefabPath == "" ? ProjectilePrefabPath : prefabPath;
        var projectile = Resources.Load<GameObject>(prefabPath);
        var projectileGameObject = Instantiate(projectile, transform.position, Quaternion.identity);
        projectileGameObject.GetComponent<AbstractProjectile>().IncomingDamage = Damage;
        return projectileGameObject;
    }
}
