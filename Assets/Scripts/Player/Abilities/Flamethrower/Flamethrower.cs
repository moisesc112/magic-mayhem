using UnityEngine;

public class Flamethrower : Ability
{
    public GameObject flamethrowerProjectile;

    private int numberOfFlameProjectiles = 10;
    private float timeBetweenProjectiles = 0.2f;
    private float currentTimeBetweenProjectiles = 0f;

    public override void Awake()
    {
        remainingDespawnTime = abilityInfo.despawnTime + (timeBetweenProjectiles * numberOfFlameProjectiles);
    }

    public override void Update()
    {
        if (numberOfFlameProjectiles > 0)
        {
            if (currentTimeBetweenProjectiles <= 0)
            {
                currentTimeBetweenProjectiles = timeBetweenProjectiles;
                CastFlamethrowerProjectile();
            }
            else
            {
                currentTimeBetweenProjectiles -= Time.deltaTime;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CastFlamethrowerProjectile()
    {
        numberOfFlameProjectiles -= 1;
        // TODO change to where player is facing
        // TODO add player velocity to projectiles?
        var direction = new Vector3(1, 0, 0);
        var projectile = Instantiate(flamethrowerProjectile, _abilitySlotsComponent.transform.position, Quaternion.identity);
        projectile.GetComponent<FlamethrowerProjectile>().castingDirection = direction;
    }
}
