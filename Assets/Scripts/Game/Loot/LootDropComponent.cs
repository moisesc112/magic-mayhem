using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropComponent : MonoBehaviour
{
	[SerializeField] LootInfo lootInfo;

	public void DropLoot()
	{
		var loot = Instantiate(lootInfo.lootGameObject, transform.position, Quaternion.identity);
		if(loot.GetComponent<Coin>() != null) 
			loot.GetComponent<Coin>().SetValue(lootInfo.amount);
		if (loot.GetComponent<HealthPotion>() != null)
			loot.GetComponent<HealthPotion>().SetValue(lootInfo.amount);
	}
}
