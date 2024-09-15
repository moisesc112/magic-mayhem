using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
	public float followSpeed = 5.0f;
	public bool lookAtTarget = true;

	private void LateUpdate()
	{
		if (target is null) return;

		transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * followSpeed);
		if (lookAtTarget)
			transform.LookAt(target.position);
	}
}
