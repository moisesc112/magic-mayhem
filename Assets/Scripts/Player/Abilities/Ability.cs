using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityInfo abilityInfo;
    protected float remainingDespawnTime;

    public virtual void Awake()
    {
        remainingDespawnTime = abilityInfo.despawnTime;
    }

    public virtual void Update()
    {
        UpdateProjectileVelocity();
        remainingDespawnTime -= Time.deltaTime;
        if (remainingDespawnTime <= 0)
        {
            Despawn();
        }
    }

    public virtual void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            Debug.Log("Hit Enemy");
            collision.GetComponent<HealthComponent>().TakeDamage(abilityInfo.damage);
            if (abilityInfo.projectileDestoryAfterCollision)
            {
                Despawn();
            }
        }
    }

    public virtual void Despawn()
    {
        Destroy(gameObject);
    }

    public virtual void UpdateProjectileVelocity()
    {
        transform.position += abilityInfo.projectileTargetDirection * abilityInfo.projectileTargetSpeed * Time.deltaTime;
    }
}
