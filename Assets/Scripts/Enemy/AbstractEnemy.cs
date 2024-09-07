using UnityEngine;

public abstract class AbstractEnemy : AbstractEntity
{
    public virtual void FindPlayer()
    {
        // TODO
    }

    public override void HandleDeath()
    {
        Debug.Log("Enemy Died");
        Destroy(gameObject);
    }
}
