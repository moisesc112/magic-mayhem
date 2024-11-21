using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
	public bool playerInNPCRange = false;
	public GameObject NPCTriggerUI;
	public GameObject NPC;

	private void Start()
	{
		NPCTriggerUI.SetActive(false);
	}

	// Trigger the ui based on if the player is inside the collider
	public virtual void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInNPCRange = true;
			if (NPC.activeSelf)
			{
				NPCTriggerUI.SetActive(true);
			}
		}
	}

	public virtual void OnTriggerStay(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			if (!NPC.activeSelf)
			{
				NPCTriggerUI.SetActive(false);
			}
		}
	}

	public virtual void OnTriggerExit(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerInNPCRange = false;
			NPCTriggerUI.SetActive(false);
		}
	}
}
