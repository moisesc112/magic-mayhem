using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
	public bool playerInShop = false;
	public GameObject ShopTriggerUI;
	public GameObject ShopKeeper;

	private void Start()
    {
		ShopTriggerUI.SetActive(false);
	}

	public virtual void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInShop = true;
			if (ShopKeeper.activeSelf)
			{
				ShopTriggerUI.SetActive(true);
			}
		}
	}

	public virtual void OnTriggerStay(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			if (!ShopKeeper.activeSelf)
			{
				ShopTriggerUI.SetActive(false);
			}
		}
	}

	public virtual void OnTriggerExit(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInShop = false;
			ShopTriggerUI.SetActive(false);
		}
	}
}
