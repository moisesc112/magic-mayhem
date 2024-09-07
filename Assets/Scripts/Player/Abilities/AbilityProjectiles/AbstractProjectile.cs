using UnityEngine;

public abstract class AbstractProjectile : MonoBehaviour
{
    public abstract float IncomingDamage { get; set; }
    public abstract bool DestroyAfterCollision { get; set; }

    public virtual void Update()
    {
        UpdateProjectileVelocity();
    }

    public virtual void OnTriggerEnter(Collider collision)
    {
        if (collision != null && collision.GetComponent<AbstractEnemy>() != null)
        {
            Debug.Log("Hit Enemy");
            collision.GetComponent<AbstractEnemy>().TakeDamage(IncomingDamage);
            if (DestroyAfterCollision)
            {
                Destroy(gameObject);
            }
        }
    }

    public virtual void UpdateProjectileVelocity()
    {
        transform.position += new Vector3(1,0,0) * Time.deltaTime;
    }
}
