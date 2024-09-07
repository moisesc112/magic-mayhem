using UnityEngine;

public abstract class AbstractProjectile : MonoBehaviour
{
    public AbilityInfo abilityInfo;

    public virtual void Update()
    {
        UpdateProjectileVelocity();
    }

    public virtual void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            Debug.Log("Hit Enemy");
            collision.GetComponent<HealthComponent>().TakeDamage(abilityInfo.damage);
            if (abilityInfo.projectileDestoryAfterCollision)
            {
                Destroy(gameObject);
            }
        }
    }

    public virtual void UpdateProjectileVelocity()
    {
        transform.position += abilityInfo.projectileTargetDirection * abilityInfo.projectileTargetSpeed * Time.deltaTime;
    }
}
