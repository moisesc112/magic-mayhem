using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : Ability
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] LayerMask layerMask;
    public GameObject chainConnector;
    private bool hit;

    private List<Transform> targets;

    private int targetIndex;
    Transform currentTarget;
    public override void Awake()
    {
        StartCoroutine(AudioFadeOut.FadeOut(audioSource, abilityInfo.despawnTime));
        hit = false;
        targets = new List<Transform>();
        base.Awake();
    }

    public override void UpdateProjectileVelocity()
    {
        transform.position += transform.forward * abilityInfo.projectileTargetSpeed * Time.deltaTime;
    }

    public override void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("hit with chain lightning");
        if (!hit && collision.tag == "GoblinEnemy")
        {
            hit = true;
            currentTarget = collision.transform;
            Collider[] tmp = Physics.OverlapSphere(collision.transform.position, 5, layerMask);

            foreach (Collider enemy in tmp)
            {
                Debug.Log("enemy in range");
                if(enemy.transform != collision.transform)
                {
                    targets.Add(enemy.transform);
                    enemy.transform.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
                    Vector3 midpoint = (collision.transform.position + enemy.transform.position) / 2;
                    // For tomorrow, try doing Vector3 midpoint = (currentTarget.position + enemy.transform.position) / 2;
                    // then, have to set currentTarget = enemy.transform, then iterate through it so it makes chain go one by one instead of all rooting from the initial collider
                    Vector3 direction = enemy.transform.position - collision.transform.position;


                    Instantiate(chainConnector, midpoint, Quaternion.LookRotation(direction));

                }
            }
            //PickTarget(collision);
        }
        base.OnTriggerEnter(collision);
    }

    public void PickTarget(Collider collision)
    {
        collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        //targets[targetIndex].GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        currentTarget = targets[targetIndex];
        targetIndex++;
        //currentTarget = targets[targetIndex];
      
    }
}
