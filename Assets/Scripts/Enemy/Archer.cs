using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavPollerComponent))]
[RequireComponent(typeof(AdvancedRootMotionNavAgent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Dissolver))]
public class Archer : MonoBehaviour
{
	[Header("Targeting")]
	[SerializeField] Transform _eyesPos;
	[SerializeField] LayerMask _targetMask;

	[Header("NavSettings")]
	[SerializeField] float _targetMinDistance;
	[SerializeField] float _targetMaxDistance;
	[SerializeField] float _sweetSpotRatio = 0.25f;

	[Header("BowSettings")]
	[SerializeField] float _maxLookAheadTime = 1.0f;
	[SerializeField] float _shootCooldown = 2.0f;
	[SerializeField] float _minTimeToDraw = 5.0f;
	[SerializeField] Transform _shootLocation;
	[SerializeField] GameObject _arrowPrefab;

	[Header("FX")]
	[SerializeField] Renderer _targetRenderer;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_poller = GetComponent<NavPollerComponent>();
		_rmNavAgent = GetComponent<AdvancedRootMotionNavAgent>();
		_rmNavAgent.targetMinDistance = _targetMinDistance;
		_rmNavAgent.targetMaxDistance = _targetMaxDistance;
		_rmNavAgent.sweetSpotRatio = _sweetSpotRatio;

		_hc = GetComponent<HealthComponent>();
		_dissolver = GetComponent<Dissolver>();
		_dissolver.SetTargetRenderer(_targetRenderer);

		_canShoot = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (!_hc.IsAlive) return;

		if (!_arrowNocked && _rmNavAgent.inDistanceRange)
			DrawArrow();

		if (_arrowNocked && _canShoot && _rmNavAgent.inDistanceRange && targetInSight())
		{
			_canShoot = false;
			ShootArrow();
		}
		else if (_arrowNocked && Mathf.Sqrt(_poller.DistanceToPlayer) > _rmNavAgent.targetMaxDistance)
		{
			_arrowNocked = false;
			StoreArrow();
		}
	}

	// Called from draw animation
	public void ArrowReady()
	{
		_arrowNocked = true;
		StartCoroutine(nameof(WaitForTimeToDraw));
	}

	// Called from recoil animation
	public void Fire()
	{
		Instantiate(_arrowPrefab, _shootLocation.position, Quaternion.LookRotation(_aimDir));
		_arrowNocked = false;
		StartCoroutine(nameof(WaitForCooldown));
	}

	private bool targetInSight()
	{
		var target = _poller.TargetPlayer;
		if (target == null) return false;

		// Calculate perdictive aiming
		var initialPos = target.GetAvatarPosition();
		var arrowSpeed = 40.0f;
		var targetVel = target.GetAvatarVelocity();
		var a = Vector3.Dot(targetVel, targetVel) - (arrowSpeed * arrowSpeed);
		var b = 2 * (Vector3.Dot(initialPos, targetVel));
		var c = Vector3.Dot(initialPos, initialPos);

		var sqrt = Mathf.Sqrt(b * b - (4 * a * c));
		var t1 = (- b + sqrt) / (2 * a);
		var t2 = (-b - sqrt) / (2 * a);

		float t;
		if (t1 > 0 && t2 > 0)
			t = Mathf.Min(t1, t2);
		else if (t1 > 0)
			t = t1;
		else if (t2 > 0)
			t = t2;
		else
			t = 0.0f;

		var dir = (initialPos + (targetVel * Mathf.Min(t, _maxLookAheadTime))) - transform.position;
		_aimDir = dir;
		// If the raycast hits something, then we don't have line of sight.
		Debug.DrawRay(_eyesPos.position, dir, Color.red);
		if (Physics.Raycast(_eyesPos.position, dir, out var hit, _targetMaxDistance, _targetMask))
		{
			return hit.collider.gameObject.tag == "Player";
		}
		else
		{
			return true;
		}

	}

	private void ShootArrow()
	{
		_animator.SetTrigger("ShootArrow");
	}

	private void DrawArrow()
	{
		_animator.SetTrigger("DrawArrow");
	}

	private void StoreArrow()
	{
		_animator.SetTrigger("StoreArrow");
	}

	IEnumerator WaitForTimeToDraw()
	{
		yield return new WaitForSeconds(_minTimeToDraw);
		_canShoot = true;
	}

	IEnumerable WaitForCooldown()
	{
		yield return new WaitForSeconds(_shootCooldown);
		DrawArrow();
	}

	NavPollerComponent _poller;
	Animator _animator;
	AdvancedRootMotionNavAgent _rmNavAgent;
	HealthComponent _hc;
	Dissolver _dissolver;

	Vector3 _aimDir;
	bool _canShoot;
	bool _arrowNocked;
}
