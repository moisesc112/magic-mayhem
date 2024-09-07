using UnityEngine;

public abstract class AbstractProjectile : MonoBehaviour
{
    public abstract float IncomingDamage { get; set; }
    public abstract bool DestroyAfterCollision { get; set; }
    public abstract Vector3 TargetDirection { get; set; }
    public abstract float TargetSpeed { get; set; }

    public virtual void Update()
    {
        UpdateProjectileVelocity();
    }

    public virtual void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.tag != "Player" && collision.GetComponent<HealthComponent>() != null)
        {
            Debug.Log("Hit Enemy");
            collision.GetComponent<HealthComponent>().TakeDamage(IncomingDamage);
            if (DestroyAfterCollision)
            {
                Destroy(gameObject);
            }
        }
    }

    public virtual void UpdateProjectileVelocity()
    {
        transform.position += TargetDirection * TargetSpeed * Time.deltaTime;
    }
}
