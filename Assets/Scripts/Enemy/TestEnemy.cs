using System;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{

    private void Awake()
    {
        _healthComp = GetComponent<HealthComponent>();

        _healthComp.onDeath += HealthComp_OnDeath;
    }

    private void OnDestroy()
    {
        _healthComp.onDeath -= HealthComp_OnDeath;
    }

    void HealthComp_OnDeath(object sender, EventArgs e)
    {
        Destroy(gameObject);
    }

    HealthComponent _healthComp;
}
