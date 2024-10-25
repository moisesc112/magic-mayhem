using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RagdollComponent))]
[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(Dissolver))]
[RequireComponent(typeof(LootDropComponent))]
public class TutorialGoblin : MonoBehaviour
{
    public Transform[] wayPoints;
    [SerializeField] Renderer _targetRenderer;

	void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        _ragdoll = GetComponent<RagdollComponent>();
        _health = GetComponent<HealthComponent>();
		_dissolver = GetComponent<Dissolver>();
        _dissolver.SetTargetRenderer(_targetRenderer);
		_lootDrop = GetComponent<LootDropComponent>();

		_health.onDeath += HealthComponent_OnDeath;
	}

	private void HealthComponent_OnDeath(object sender, System.EventArgs e)
	{
        StartCoroutine(nameof(HandleDeath));
	}

	// Start is called before the first frame update
	void Start()
    {
		_spawnPoint = gameObject.transform.position;
        Init();
	}

    void Init()
    {
        gameObject.transform.position = _spawnPoint;
        gameObject.transform.rotation = Quaternion.identity;
        _currentTargetWaypoint = 0;

        _ragdoll.DisableRagdoll();
        _dissolver.ResetEffect(deactivateGameObject: false);
        _agent.enabled = true;

        _health.health = _health.maxHealth;
        StartCoroutine(nameof(GoToWayPoint));
	}

	IEnumerator GoToWayPoint()
    {
        if (wayPoints.Length == 0)
            yield break;

		var target = wayPoints[_currentTargetWaypoint];
		_agent.destination = target.position;
		yield return new WaitUntil(() => (target.position - transform.position).magnitude < 0.5f);
		_currentTargetWaypoint = ++_currentTargetWaypoint % wayPoints.Length;
        yield return GoToWayPoint();
    }

    IEnumerator HandleDeath()
    {
        StopCoroutine(nameof(GoToWayPoint));
        _ragdoll.EnableRagdoll();
        _dissolver.StartDissolving();
        _health.health = 0;

        _agent.enabled = false;
        _lootDrop.DropLoot();
        yield return new WaitForSeconds(5.0f);
        Init();
	}

    NavMeshAgent _agent;
    Animator _anim;
    RagdollComponent _ragdoll;
    HealthComponent _health;
	Dissolver _dissolver;
	LootDropComponent _lootDrop;

	int _currentTargetWaypoint = 0;
    Vector3 _spawnPoint;
}
