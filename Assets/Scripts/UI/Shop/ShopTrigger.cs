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


	// Trigger the ui based on if the player is inside the collider
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
