using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public abstract class AbstractTrap : MonoBehaviour
{
    public TrapInfo trapInfo;
    private List<Collider> trackedEnemies = new List<Collider>();
    [SerializeField] private Animator myTrap = null;


    public void ActivateTrap()
    {
        trapInfo.isActivated = true;
        Debug.Log("Trap has been activated");
        StartCoroutine(TrapActivationDuration());
    }

    public virtual IEnumerator TrapActivationDuration()
    {
        yield return new WaitForSeconds(trapInfo.activeDuration);
        trapInfo.isActivated = false;
        myTrap.SetTrigger("ResetTrap");
        Debug.Log("trap timed out");
    }


    public virtual void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && trapInfo.isActivated)
        {
            Debug.Log("We got one");
            trapInfo.isSprung = true;
            trackedEnemies.Add(collision);
            if (trackedEnemies.Count == 1)
            {
                myTrap.SetTrigger("TriggerTrap");
            }
        }
        else
        {
            //Debug.Log(collision);
        }
    }

    public virtual void OnTriggerStay(Collider collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && trapInfo.isActivated)
        {
            if (trapInfo.isSprung)
            {
                collision.GetComponent<HealthComponent>().TakeDamage(trapInfo.damage);
                trapInfo.isSprung = false;
                StartCoroutine(UseTrapCoroutine());
            }
        }
        else
        {
            //Debug.Log("Player still in Trap");
        }
    }

    //Cooldown for enemy taking damage
    //If its a fire trap, this functionality would go into the enemy's class
    //If a spike trap, this function would stay in Trap's class
    public virtual IEnumerator UseTrapCoroutine()
    {
        yield return new WaitForSeconds(trapInfo.damageCooldown);
        trapInfo.isSprung = true;
    }

    public virtual void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            //Debug.Log("Enemy left trap");
            trapInfo.isSprung = true;
            trackedEnemies.Remove(collision);
            if (trackedEnemies.Count == 0)
            {
                myTrap.SetTrigger("ResetTrap");
            }
        }
        else
        {
            //Debug.Log("Player left Trap");
        }
    }
    void Update()
    {
        for (int i = trackedEnemies.Count - 1; i >= 0; i--)
        {
            // if game object is disabled in hierarchy
            if (!trackedEnemies[i].gameObject.activeInHierarchy) 
            {
                trackedEnemies.RemoveAt(i);
                Debug.Log("Enemy destroyed while in trigger");
            }
            if (trackedEnemies.Count==0)
            {
                myTrap.SetTrigger("ResetTrap");
            }
        }
    }
}
