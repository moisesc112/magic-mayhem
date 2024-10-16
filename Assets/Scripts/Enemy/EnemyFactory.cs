using UnityEngine;
using static EnemyContent;

[RequireComponent(typeof(GoblinPool))]
[RequireComponent(typeof(GolemPool))]
[RequireComponent(typeof(ArcherPool))]
[RequireComponent(typeof(WarChiefPool))]
public class EnemyFactory : MonoBehaviour
{
	void Awake()
	{
		_goblinPool = GetComponent<GoblinPool>();
		_golemPool = GetComponent<GolemPool>();
		_archerPool = GetComponent<ArcherPool>();
		_warChiefPool = GetComponent<WarChiefPool>();
	}

	public void SpawnEnemy(EnemyToSpawn enemy, Vector3 location)
	{
		switch (enemy)
		{
			case EnemyToSpawn.Goblin:
				{
					var goblin = _goblinPool.pool.Get();
					goblin.InitFromPool(location, (goblin) => _goblinPool.pool.Release(goblin));
					goblin.gameObject.SetActive(true);
					goblin.enabled = true;
					break;
				}
			case EnemyToSpawn.WarChief:
				{
					var warchief = _warChiefPool.pool.Get();
					warchief.InitFromPool(location, (goblin) => _warChiefPool.pool.Release(goblin));
					warchief.gameObject.SetActive(true);
					warchief.enabled = true;
					break;
				}
			case EnemyToSpawn.Golem:
				{
					var golem = _golemPool.pool.Get();
					golem.InitFromPool(location, (golem) => _golemPool.pool.Release(golem));
					golem.gameObject.SetActive(true);
					golem.enabled = true;
					break;
				}
			case EnemyToSpawn.Archer:
				{
					var archer = _archerPool.pool.Get();
					archer.InitFromPool(location, (archer) => _archerPool.pool.Release(archer));
					archer.gameObject.SetActive(true);
					archer.enabled = true;
					break;
				}
			default:
				Debug.LogError($"ATTEMPTING TO SPAWN UNKNOWN ENEMY: {enemy}");
				break;
		}
	}

	GoblinPool _goblinPool;
	GolemPool _golemPool;
	WarChiefPool _warChiefPool;
	ArcherPool _archerPool;
}
