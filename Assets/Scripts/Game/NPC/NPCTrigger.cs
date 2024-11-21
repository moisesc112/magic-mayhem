using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
	public bool playerTalkingToNPC = false;
	public GameObject ring;

    // Trigger the ui based on if the player is inside the collider
    public virtual void OnTriggerEnter(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerTalkingToNPC = true;
			ring.SetActive(true);

		}
	}

	public virtual void OnTriggerExit(Collider collision)
	{
		if (collision.CompareTag("Player"))
		{
			playerTalkingToNPC = false;
			ring.SetActive(false);
		}
	}
}
