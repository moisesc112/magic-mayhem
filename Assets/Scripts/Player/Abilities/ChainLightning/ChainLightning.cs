using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : Ability
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] LayerMask layerMask;

    public GameObject chainConnector;
    public int chainConnectorDetectionRadius;
    public int chainConnectorTargetCap;

    private bool hit;
    private Vector3 midpoint;
    private Vector3 direction;
    private float distance;
    private List<Transform> targets;
    private int targetIndex;
    private Transform currentTarget;

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
        if (!hit && collision != null && !collision.CompareTag("Player") && collision.GetComponent<HealthComponent>() != null)
        {
            hit = true;
            Collider[] chainConnectorRange = Physics.OverlapSphere(collision.transform.position, chainConnectorDetectionRadius, layerMask);

            foreach (Collider enemy in chainConnectorRange)
            {
                if(enemy.transform != collision.transform)
                {
                    targets.Add(enemy.transform);
                }
            }
            if (targets.Count > 0)
            {
                collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
                PickTarget(collision);
            }
            else
            {
                OnlyOriginalTarget(collision);
            }
        }
    }

    public void OnlyOriginalTarget(Collider collision)
    {
        collision.GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        Despawn();
    }

    public void PickTarget(Collider collision)
    {
        midpoint = (collision.transform.position + targets[targetIndex].position) / 2;
        direction = collision.transform.position - targets[targetIndex].position;
        distance = direction.magnitude;

        var chainConnectorInstance = Instantiate(chainConnector, midpoint, Quaternion.LookRotation(direction));
        chainConnectorInstance.transform.localScale = new Vector3(1, 1, distance);

        targets[targetIndex].GetComponent<HealthComponent>().TakeDamage(GetAbilityDamage());
        currentTarget = targets[targetIndex];
        targetIndex++;
    }

    public override void Update()
    {
        if (currentTarget != null && hit && targetIndex < chainConnectorTargetCap && targetIndex < targets.Count)
        {
            PickTarget(currentTarget.GetComponent<Collider>());
        }
        else if (hit && targets.Count > 0)
        {
            Despawn();
        }
        base.Update();
    }
}
