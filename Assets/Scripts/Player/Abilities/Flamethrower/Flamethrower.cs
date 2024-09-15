using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    public GameObject flamethrowerProjectile;

    private int numberOfFlameProjectiles = 10;
    private float timeBetweenProjectiles = 0.2f;
    private float currentTimeBetweenProjectiles = 0f;

    public void Update()
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
        var direction = new Vector3(1, 0, 0);
        //transform.position = PlayerManager.instance.PlayerControllers[0]..transform.position;
        var projectile = Instantiate(flamethrowerProjectile, transform.position, Quaternion.identity);
        projectile.GetComponent<FlamethrowerProjectile>().castingDirection = direction;
    }
}
