using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityExtensions;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavPollerComponent))]
public class EnemyBase : MonoBehaviour
{
	[SerializeField] RefreshableComponent[] _additionalRefreshableComponents;

	void Awake()
	{
		_healthComponent = GetComponent<HealthComponent>();
		_healthComponent.onDeath += HealthComp_OnDeath;
		
		_agent = GetComponent<NavMeshAgent>();
		_navPoller = GetComponent<NavPollerComponent>();

		_refreshableComponents = gameObject.GetAllComponents<RefreshableComponent>();
		if (_additionalRefreshableComponents?.Length > 0)
			_refreshableComponents = _refreshableComponents.Concat(_additionalRefreshableComponents).ToArray();
		_refreshableComponents = _refreshableComponents.Distinct().ToArray();
		DoAwake();
	}

	// Start is called before the first frame update
	void Start()
	{
		DoStart();
	}

	// Update is called once per frame
	void Update()
	{
		if (_healthComponent.IsAlive == false) return;

		DoUpdate();
	}

	void OnDestroy()
	{
		_healthComponent.onDeath -= HealthComp_OnDeath;
	}

	protected virtual void DoAwake() { }
	protected virtual void DoStart() { }
	protected virtual void DoUpdate() { }

	public void InitFromPool(Vector3 location, Action<EnemyBase> releaseAction)
	{
		transform.position = location;
		_agent.transform.position = location;

		enabled = true;
		_agent.enabled = true;

		_healthComponent.health = _healthComponent.maxHealth;

		if (_refreshableComponents != null)
		{
			foreach (var comp in _refreshableComponents)
			{
				comp.OnInit();
			}
		}

		SelfOnInit();

		if (_releaseToPoolAction is null)
			_releaseToPoolAction = releaseAction;
	}

	protected virtual IEnumerator SelfOnInit() { yield return null; }
	protected virtual IEnumerator SelfOnKilled() { yield return null; }

	private void HealthComp_OnDeath(object sender, EventArgs e)
	{
		StartCoroutine(nameof(HandleDeath));
	}

	IEnumerator HandleDeath()
	{
		enabled = false;

		WaveManager.instance?.ReportEnemyKilled();

		foreach (var comp in _refreshableComponents)
		{
			comp.OnKilled();
		}
		
		yield return SelfOnKilled();

		yield return new WaitForSeconds(3.0f);
		_releaseToPoolAction?.Invoke(this);
	}

	protected HealthComponent _healthComponent;
	protected NavMeshAgent _agent;
	protected NavPollerComponent _navPoller;
	protected RefreshableComponent[] _refreshableComponents;
	Action<EnemyBase> _releaseToPoolAction;
}
