using System.Collections;
using UnityEngine;

public abstract class AbstractTrap : MonoBehaviour
{
    public TrapInfo trapInfo;

    //TODO Replace activating OnTriggerEnter with player activating trap through interact button

    public virtual void OnTriggerEnter(Collider collision)
    {

        if (collision.tag != "Player")
        { 
            trapInfo.isActive = true;  
        }
        else
        {
            Debug.Log("Player Entered Trap");
        }
    }

    public virtual void OnTriggerStay(Collider collision)
    {

        if (collision.tag != "Player")
        {
            if (trapInfo.isActive)
            {
                Debug.Log("Enemy Taking Trap Damage");
                collision.GetComponent<HealthComponent>().TakeDamage(trapInfo.damage);
                trapInfo.isActive = false;
                StartCoroutine(UseTrapCoroutine());
            }
        }
        else
        {
            Debug.Log("Player still in Trap");
        }
    }

    //Cooldown for enemy taking damage
    //If its a fire trap, this functionality would go into the enemy's class
    //If a spike trap, this function would stay in Trap's class
    public virtual IEnumerator UseTrapCoroutine()
    {
        yield return new WaitForSeconds(trapInfo.cooldown);
        trapInfo.isActive = true;
    }

    public virtual void OnTriggerExit(Collider collision)
    {
        if (collision.tag != "Player")
        {
            Debug.Log("Enemy left trap");
            trapInfo.isActive = true;
        }
        else
        {
            Debug.Log("Player left Trap");
        }
    }
}
