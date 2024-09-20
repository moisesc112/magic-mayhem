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

	public void UpdateFps(float value) => fps.text = $"FPS: {value}";
	public void UpdateTrackedHealth(float value) => trackedHealth.text = $"Health: {value}";
	public void UpdateTrackedVelocity(Vector2 vel) => trackedVelocity.text = $"Velocity x:{vel.x}, z:{vel.y}";
}
