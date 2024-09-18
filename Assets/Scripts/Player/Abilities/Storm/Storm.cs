using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Storm : Ability
{
    private float damageIntervalTime = 0.2f;
    private float remainingDamageIntervalTime;
    private List<Collider> damagableCollidersInsideAbility = new List<Collider>();

    public override void Update()
    {
        if (remainingDamageIntervalTime <= 0)
        {
            remainingDamageIntervalTime = damageIntervalTime;
            damagableCollidersInsideAbility.RemoveAll(x => x.IsDestroyed());
            damagableCollidersInsideAbility.ForEach(x => x.GetComponent<HealthComponent>().TakeDamage(abilityInfo.damage));
        }
        remainingDamageIntervalTime -= Time.deltaTime;
        base.Update();
    }

    public override void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            damagableCollidersInsideAbility.Add(collision);
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null && damagableCollidersInsideAbility.Contains(collision))
        {
            damagableCollidersInsideAbility.Remove(collision);
        }
    }

    public override void UpdateProjectileVelocity()
    {
        transform.position = _player.GetAvatarPosition();
    }
}
