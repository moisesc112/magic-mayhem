using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChainConnector : MonoBehaviour
{
    public float remainingDespawnTime;

    void Update()
    {
        remainingDespawnTime -= Time.deltaTime;
        if (remainingDespawnTime <= 0)
        {
            Destroy(gameObject);
        }
    }

}
