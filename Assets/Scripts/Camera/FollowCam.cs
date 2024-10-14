using UnityEngine;

public class FollowCam : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;

	// set private variables for boundaries
	private float xMinBoundary;
	private float xMaxBoundary;
	private float zMinBoundary;
	private float zMaxBoundary;

	public float followSpeed = 5.0f;
	public bool lookAtTarget = true;

	private void Start()
    {	
		// If a levelmanager instance is in the scene use the bounds
		// otherwise default to a large number for the bounds
		if(LevelManager.instance != null)
        {
			xMinBoundary = LevelManager.instance.xMinBoundary;
			xMaxBoundary = LevelManager.instance.xMaxBoundary;
			zMinBoundary = LevelManager.instance.zMinBoundary;
			zMaxBoundary = LevelManager.instance.zMaxBoundary;
		}
        else
        {
			xMinBoundary = -1000;
			xMaxBoundary = 1000;
			zMinBoundary = -1000;
			zMaxBoundary = 1000;
		}
	}
	
	
	private void LateUpdate()
	{
		if (target is null) return;

		Vector3 camPosition = target.position + offset;

		// clamp the camera position according to the x,z bounds
		float clampedX = Mathf.Clamp(camPosition.x, xMinBoundary, xMaxBoundary);
		float clampedZ = Mathf.Clamp(camPosition.z, zMinBoundary, zMaxBoundary);

		// Give a new camera position with the bounds
		camPosition = new Vector3(clampedX, camPosition.y, clampedZ);

		transform.position = Vector3.Lerp(transform.position, camPosition, Time.deltaTime * followSpeed);
		if (lookAtTarget)
			transform.LookAt(target.position);
	}
}
