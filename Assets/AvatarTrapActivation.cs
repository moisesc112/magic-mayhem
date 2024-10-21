using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarTrapActivation : MonoBehaviour
{
	public void Awake()
    {
		_player = GetComponentInParent<Player>();
    }
	private void OnTriggerEnter(Collider collision)
	{
		if (collision.tag == "Trap")
		{
			Debug.Log("Player Entered Trap");
			_trap = collision.GetComponent<AbstractTrap>();
			if(_player != null && _trap != null)
			{
				_player.setDetectedTrap(_trap);
            }
		}
	}
	private void OnTriggerExit(Collider collision)
	{
		if (collision.tag == "Trap")
		{
			Debug.Log("Player Left Trap");
			_trap = null;
			if (_player != null)
			{
				_player.setDetectedTrap(_trap);

			}
		}
	}

	Player _player;
	AbstractTrap _trap;
}
