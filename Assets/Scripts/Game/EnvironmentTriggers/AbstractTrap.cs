using System.Collections;
using UnityEngine;

public abstract class AbstractTrap : MonoBehaviour
{
    public TrapInfo trapInfo;

    //refer to TrapInfo about why this is here
    private bool isActive = true;

    public virtual void OnTriggerStay(Collider collision)
    {

        if (collision.tag != "Player")
        {
            if (isActive)
            {
                Debug.Log("Enemy Taking Trap Damage");
                collision.GetComponent<HealthComponent>().TakeDamage(trapInfo.damage);
                isActive = false;
                StartCoroutine(UseTrapCoroutine());
            }
        }
        else
        {
            Debug.Log("Player Entered Trap");
        }
    }

    //Cooldown for enemy taking damage
    //If its a fire trap, this functionality would go into the enemy's class
    //If a spike trap, this function would stay in Trap's class
    public virtual IEnumerator UseTrapCoroutine()
    {
        yield return new WaitForSeconds(trapInfo.cooldown);
        isActive = true;
    }

    public virtual void OnTriggerExit(Collider collision)
    {
        if (collision.tag != "Player")
        {
            Debug.Log("Enemy left trap");
            isActive = true;
        }
        else
        {
            Debug.Log("Player left Trap");
        }
    }
}
