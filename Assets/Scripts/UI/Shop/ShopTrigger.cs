using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
	public bool playerInShop = false;
	public GameObject ShopKeeper;

	// Trigger the ui based on if the player is inside the collider
	public virtual void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInShop = true;
			var player = collision.gameObject.GetComponentInParent<Player>();
			if (player)
				player.SetPromptText(_openShopTextFormat, ActionToTextMapper.PlayerInputAction.OPENSTORE);
		}
	}

	public virtual void OnTriggerExit(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInShop = false;
			var player = collision.gameObject.GetComponentInParent<Player>();
			if (player)
				player.ClearPromptText();
		}
	}

	string _openShopTextFormat = "Press {0} to open shop.";
}
