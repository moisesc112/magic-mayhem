using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropComponent : MonoBehaviour
{
    [SerializeField] LootInfo lootInfo;

    public void DropLoot()
    {
        Instantiate(lootInfo.lootGameObject, transform.position, Quaternion.identity);
    }
}
