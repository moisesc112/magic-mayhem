using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class LootDropComponent : MonoBehaviour
{
    [SerializeField] LootInfo lootInfo;
    private HealthComponent healthComponent;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.onDeath += HealthComponent_onDeath;
    }

    private void OnDestroy()
    {
        healthComponent.onDeath -= HealthComponent_onDeath;
    }

    private void HealthComponent_onDeath(object sender, System.EventArgs e)
    {
        DropLoot();
    }

    public void DropLoot()
    {
        Instantiate(lootInfo.lootGameObject, transform.position, Quaternion.identity);
    }
}
