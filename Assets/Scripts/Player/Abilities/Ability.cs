using UnityEngine;

public class Ability : MonoBehaviour
{
    public AbilityInfo abilityInfo;
    protected float remainingDespawnTime;
    protected Vector3 aimDirection;

    public virtual void Awake()
    {
        remainingDespawnTime = abilityInfo.despawnTime;
    }

    public virtual void Start()
    {
        SetDirectionToPlayerAimDirection();
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

    public virtual Vector3 SetDirectionToPlayerAimDirection()
    {
        if (_player != null)
        {
            var playerAimDirection = _player.GetAimDirection();
            aimDirection = new Vector3(playerAimDirection.x, 0, playerAimDirection.y);
            return aimDirection;
        }
        Debug.LogWarning("Player not set in ability");
        return Vector3.zero;
    }

    public virtual void UpdateProjectileVelocity()
    {
        transform.position += aimDirection * abilityInfo.projectileTargetSpeed * Time.deltaTime;
    }

    public void SetPlayer(Player player) => _player = player;

    protected Player _player;
}
