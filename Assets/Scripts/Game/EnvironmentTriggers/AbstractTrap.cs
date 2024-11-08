using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public abstract class AbstractTrap : MonoBehaviour
{
    public TrapInfo trapInfo;
    public GameObject TrapTriggerUI;
    private List<Collider> trackedEnemies = new List<Collider>();
    [SerializeField] private Animator myTrap = null;

    private void Start()
    {
        TrapTriggerUI.SetActive(false);
    }

    public void ActivateTrap()
    {
        trapInfo.isActivated = true;
        Debug.Log("Trap has been activated");
        StartCoroutine(TrapActivationDuration());
    }

    public virtual IEnumerator TrapActivationDuration()
    {
        // Keep trap active for a certain duration and
        // then disable trap and reset animation
        yield return new WaitForSeconds(trapInfo.activeDuration);
        trapInfo.isActivated = false;
        myTrap.SetTrigger("ResetTrap");
        Debug.Log("Trap activation has expired");
    }


    public virtual void OnTriggerEnter(Collider collision)
    {
        // If enemy is in trap and the trap is activated do the animation
        // and add the enemy to list of enemies in trap
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && trapInfo.isActivated)
        {
            trapInfo.isSprung = true;
            trackedEnemies.Add(collision);
            if (trackedEnemies.Count == 1)
            {
                myTrap.SetTrigger("TriggerTrap");
            }
        }
    }

    public virtual void OnTriggerStay(Collider collision)
    {
        // If enemy is in trap and the trap is activated do the animation
        // and the trap animation has been sprung then handle the damage and start
        // trap damage cooldown
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") && trapInfo.isActivated)
        {
            if (trapInfo.isSprung)
            {
                collision.GetComponent<HealthComponent>().TakeDamage(trapInfo.damage);
                trapInfo.isSprung = false;
                StartCoroutine(UseTrapCoroutine());
            }
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
        // enemy leaves trap and there are no longer any more enemies
        // in trap then reset the trap animation
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            trapInfo.isSprung = true;
            trackedEnemies.Remove(collision);
            if (trackedEnemies.Count == 0)
            {
                myTrap.SetTrigger("ResetTrap");
            }
        }
    }
    void Update()
    {
        for (int i = trackedEnemies.Count - 1; i >= 0; i--)
        {
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
