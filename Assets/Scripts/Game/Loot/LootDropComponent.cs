using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LootDropComponent : MonoBehaviour
{
    [SerializeField] LootInfo lootInfo;

    public void DropLoot()
    {
        Instantiate(lootInfo.lootGameObject, transform.position, Quaternion.identity);
    }
}
