using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIDebugUtility : MonoBehaviour
{
	public static UIDebugUtility instance { get; private set; }

	public TextMeshProUGUI trackedVelocity;
	public TextMeshProUGUI trackedHealth;
	public TextMeshProUGUI fps;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}

		instance = this;

		DontDestroyOnLoad(gameObject);
	}

	public void UpdateFps(float value)
	{
		if (fps is null) return;
			fps.text = $"FPS: {value}";
	}

	public void UpdateTrackedHealth(float value)
	{
		if (trackedHealth is null) return;

		trackedHealth.text = $"Health: {value}";
	}
	public void UpdateTrackedVelocity(Vector2 vel)
	{
		if (trackedVelocity is null) return;
		trackedVelocity.text = $"Velocity x:{vel.x}, z:{vel.y}";
	}

}
