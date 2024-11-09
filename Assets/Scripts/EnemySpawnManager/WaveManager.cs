using System;
using System.Collections;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

[RequireComponent (typeof(WaveInfo))]
[RequireComponent(typeof(AudioSource))]
public class WaveManager : MonoBehaviour
{
	public static WaveManager instance { get; private set; }

	public int timeBeforeGameStarts;
	public GameObject shopkeeper;

	[SerializeField] EnemyFactory _enemyFactory;
	[SerializeField] bool inTestingScene;
	[SerializeField] Transform[] _spawnLocations;

	[System.NonSerialized] public bool inPlaceholderScene = true;
	[System.NonSerialized] public bool gameStarted;
	[System.NonSerialized] public bool inWaveCooldown;
	[System.NonSerialized] public bool inGameStartCooldown;
	[System.NonSerialized] public bool isGameFinished;
	[System.NonSerialized] public bool shouldSkipWaveCooldown;
	[System.NonSerialized] public int currentWaves;
	[System.NonSerialized] public int groupKillCount;
	[System.NonSerialized] public int enemiesAlive;
	[System.NonSerialized] public int totalEnemiesPerWave;
	[System.NonSerialized] public float timeBetweenWaves;

	private int aliveEnemies;
	private int enemiesToSpawn;
	private WaveInfo waveInfo;
	private AudioSource _audioSource;
	private WaveInfo.WaveContent[] waves;

	public event EventHandler<GameStartedEventArgs> gameStarting;
	public event EventHandler<WaveStartedEventArgs> waveStarted;
	public event EventHandler<WaveEndedEventArgs> waveFinished;
	public event EventHandler<EnemyDiedEventArgs> enemyDied;

	void Start()
	{
		isGameFinished = false;
		DisableShopDuringWave();
	}

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
			return;
		}

		instance = this;

		waveInfo = GetComponent<WaveInfo>();
		_audioSource = GetComponent<AudioSource>();
		waves = waveInfo.GetWaveContents();
		_inGameMenu = FindObjectOfType<InGameMenu>();
	}

    public void StartGame() => gameStarting?.Invoke(this, new GameStartedEventArgs(timeBeforeGameStarts));
	
	public void SpawnWaves()
	{
		StartCoroutine(DoSpawnWaves());
	}

	IEnumerator DoSpawnWaves()
	{
		var waveNum = 0;
		var groupNum = 0;
		foreach (var wave in waves)
		{
			groupNum = 0;
			Debug.Log("Spawning wave");
			var enemyCount = wave.GetEnemyCount();
			enemiesAlive = enemyCount;
			aliveEnemies = 0;
			_audioSource.Play();
			waveStarted?.Invoke(this, new WaveStartedEventArgs(waveNum + 1, enemyCount));
			DisableShopDuringWave();
			foreach (var group in wave.groups)
			{
				aliveEnemies += group.GetEnemyCount();
				Debug.Log("Spawning group");
				var location = _spawnLocations[UnityRandom.Range(0, _spawnLocations.Length)];
				foreach (var enemy in group.enemySpawn)
				{
					for (int i = 0; i < enemy.amountToSpawn; i++)
					{
						var randomLoc = UnityRandom.insideUnitCircle * 3;
						var finalLoc = location.position + new Vector3(randomLoc.x, 0, randomLoc.y);
						_enemyFactory.SpawnEnemy(enemy.enemyToSpawn, finalLoc);
						yield return new WaitForSeconds(wave.timeBetweenEnemies);
					}
				}
				groupNum++;
				yield return new WaitForSecondsOrCondition(
					condition: () => groupNum == wave.groups.Length || aliveEnemies == 0,
					seconds: group.timeToNextGroup);
			}
			Debug.Log("Waiting for all enemies to be dead.");
			yield return new WaitUntil(() => enemiesAlive == 0);
			Debug.Log("Enemies dead");
			if (++waveNum != waves.Length)
			{
				waveFinished?.Invoke(this, new WaveEndedEventArgs(wave.timeToNextWave));
				EnableShopAfterWave();
				yield return new WaitForSecondsOrCondition(
					condition: () => shouldSkipWaveCooldown,
					seconds: wave.timeToNextWave);
				shouldSkipWaveCooldown = false;
			}
		}

		Debug.Log("You win!");
		StartCoroutine(nameof(GameWin));
	}

	IEnumerator GameWin()
	{
		// Handle Win Menu
		yield return new WaitForSeconds(1.5f);
		foreach (PlayerController playerController in PlayerManager.instance.PlayerControllers)
		{
			playerController.playerInput.actions.FindActionMap("UI").Enable();
		}
		_inGameMenu.WinGameMenu();
	}

	public void ReportEnemyKilled()
	{
		enemiesAlive--;
		aliveEnemies--;
		enemyDied?.Invoke(this, new EnemyDiedEventArgs(enemiesAlive));
	}

	public void DisableShopDuringWave()
	{
		if (!inTestingScene)
		{
			inWaveCooldown = false;
			foreach (PlayerController playerController in PlayerManager.instance.PlayerControllers)
			{
				playerController.ForceCloseActiveShopUI(playerController);
			}
			// Deactivate Shopkeeper
			if (shopkeeper != null)
            {
				shopkeeper.SetActive(false);
            }
		}
	}

	void EnableShopAfterWave()
	{
		inWaveCooldown = true;
		foreach (PlayerController playerController in PlayerManager.instance.PlayerControllers)
		{
			playerController.playerInput.actions.FindAction("OpenShop").Enable();
		}
		// Activate Shopkeeper
		if (shopkeeper != null)
		{
			shopkeeper.SetActive(true);
		}
	}

	public void SkipShopPhase()
    {
		Debug.Log("Skipping Shop Phase");
		shouldSkipWaveCooldown = true;
    }

	InGameMenu _inGameMenu;
}

public sealed class WaveStartedEventArgs : EventArgs
{
	public WaveStartedEventArgs(int num, int count)
	{
		waveNum = num;
		enemyCount = count;
	}
	public int waveNum { get; }
	public int enemyCount { get; }
}

public sealed class WaveEndedEventArgs : EventArgs
{
	public WaveEndedEventArgs(int time)
	{
		timeTillNextWave = time;
	}
	public int timeTillNextWave { get; }
}

public sealed class GameStartedEventArgs : EventArgs
{
	public GameStartedEventArgs(int count)
	{
		countDown = count;
	}
	public int countDown { get; }
}

public sealed class EnemyDiedEventArgs : EventArgs
{
	public EnemyDiedEventArgs(int count)
	{
		remainingEnemies = count;
	}
	public int remainingEnemies { get; }
}