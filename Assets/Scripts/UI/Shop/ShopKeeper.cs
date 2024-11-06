using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
	public static ShopKeeper instance { get; private set; }
	public Transform shopkeeperPosition;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}

		instance = this;
	}
}