using UnityEngine;

public class FollowCam : MonoBehaviour
{
	public Transform target;
	public Vector3 offset;
	public float xMinBoundary;
	public float xMaxBoundary;
	public float followSpeed = 5.0f;
	public bool lookAtTarget = true;

	private void LateUpdate()
	{
		if (target is null) return;

		Vector3 camPosition = target.position + offset;

		float clampedX = Mathf.Clamp(camPosition.x, xMinBoundary, xMaxBoundary);


		camPosition = new Vector3(clampedX, camPosition.y, camPosition.z);

		transform.position = Vector3.Lerp(transform.position, camPosition, Time.deltaTime * followSpeed);
		if (lookAtTarget)
			transform.LookAt(target.position);
	}
}
