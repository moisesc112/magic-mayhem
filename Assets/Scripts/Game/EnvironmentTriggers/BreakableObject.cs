using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LootDropComponent), typeof(Rigidbody), typeof(CapsuleCollider))]
public class BreakableObject : MonoBehaviour
{
    private HealthComponent healthComponent;
    [SerializeField] private GameObject unbrokenGameObject;
    [SerializeField] private GameObject brokenGameObject;
    [SerializeField] private float despawnTime;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.onDeath += HealthComponent_OnDeath;
    }

    private void OnDestroy()
    {
        healthComponent.onDeath -= HealthComponent_OnDeath;
    }

    private void HealthComponent_OnDeath(object sender, EventArgs e)
    {
        healthComponent.onDeath -= HealthComponent_OnDeath;
        GetComponent<Collider>().enabled = false;
        unbrokenGameObject.SetActive(false);
        brokenGameObject.SetActive(true);
        StartCoroutine(DespawnCoroutine());
    }


    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}