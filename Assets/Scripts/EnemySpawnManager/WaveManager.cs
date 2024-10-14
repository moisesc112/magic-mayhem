using System.Collections;
using UnityEngine;

[RequireComponent (typeof(WaveInfo))]
public class WaveManager : MonoBehaviour
{
    public static WaveManager instance { get; private set; }

    public float timeBeforeGameStarts;

    [System.NonSerialized] public bool inPlaceholderScene = true;
    [System.NonSerialized] public bool gameStarted;
    [System.NonSerialized] public bool inWaveCooldown;
    [System.NonSerialized] public bool inGameStartCooldown;
    [System.NonSerialized] public bool isGameFinished;
    [System.NonSerialized] public int currentWaves;
    [System.NonSerialized] public int groupKillCount;
    [System.NonSerialized] public int enemiesAlive;
    [System.NonSerialized] public int totalEnemiesPerWave;
    [System.NonSerialized] public float timeBetweenWaves;

    private bool inEnemyCooldown;
    private bool inGroupCooldown;
    private bool isSettingEnemy;
    private int waveIterator;
    private int groupIterator;
    private int enemyIterator;
    private int enemiesForCurrentGroup;
    private int enemiesToSpawn;
    private Coroutine groupCoroutine;
    private WaveInfo waveInfo;
    private WaveInfo.WaveContent[] waves;
   
    void Start()
    {
        waveInfo = GetComponent<WaveInfo>();
        waves = waveInfo.GetWaveContents();
        isGameFinished = false;
       
    }

    void Awake()
    {
        instance = this;
        if (inPlaceholderScene)
        {
            StartCoroutine(GameStartCooldown(timeBeforeGameStarts));
        }
    }

    void Update()
    {
        if (gameStarted && !isGameFinished)
        {
            SpawnWave();
        }
        else if (gameStarted && isGameFinished)
        {
            // Win Condition
        }
    }

    // Might want to change this to be in a Game State Manager Class
    public static void OnSceneLoaded(string scene)
    {
        if (scene == "Testing")
        {
            instance.inPlaceholderScene = true;
        }
        else
        {
            Debug.LogWarning("Wrong Scene, current scene is: " + scene);
        }
    }

    void SpawnWave()
    {
        if (waveIterator < waves.Length && !inEnemyCooldown && !inGroupCooldown && !inWaveCooldown)
        {
            var currentWave = waves[waveIterator];
            timeBetweenWaves = currentWave.GetTimeToNextWave();

            if (groupIterator < currentWave.groups.Length)
            {
                var currentGroup = currentWave.groups[groupIterator];
                var timeBetweenGroups = currentGroup.GetTimeToNextGroup();
                if (enemyIterator < currentGroup.enemySpawn.Length)
                {
                    var currentEnemy = currentGroup.enemySpawn[enemyIterator];
                    var enemyToSpawn = currentEnemy.enemyToSpawn.ToString();
                    var amountToSpawn = currentEnemy.GetAmountToSpawn();
                    var timeBetweenEnemies = currentWave.GetTimeBetweenEnemies();

                    if (!isSettingEnemy)
                    {
                        enemiesToSpawn = amountToSpawn;
                        isSettingEnemy = true;
                    }
                    if (enemiesToSpawn > 0)
                    {
                        ObjectPooler.SpawnEnemy(enemyToSpawn);
                        enemiesToSpawn--;

                        if(enemiesToSpawn != 0 || enemyIterator != currentGroup.enemySpawn.Length - 1)
                        {
                            StartCoroutine(EnemyCooldown(timeBetweenEnemies));
                        }
                    }
                    else
                    {
                        enemyIterator++;
                        isSettingEnemy = false;
                    }
                }
                else if (enemiesToSpawn == 0 && !inEnemyCooldown && !inGroupCooldown)
                {
                    enemyIterator = 0;
                    groupIterator++;
                    groupCoroutine = StartCoroutine(GroupCooldown(timeBetweenGroups));
                }
            }
            else
            {
                if (enemiesAlive == 0 && waveIterator != waves.Length - 1)
                {
                    enemyIterator = 0;
                    groupIterator = 0;
                    totalEnemiesPerWave = 0;
                    groupKillCount = 0;
                    waveIterator++;
                    StartCoroutine(WaveCooldown(timeBetweenWaves));
                }
                else if (enemiesAlive == 0 && waveIterator == waves.Length - 1)
                {
                    isGameFinished = true;
                }
                
            }
        }
        else if (inGroupCooldown && !inWaveCooldown && groupKillCount == enemiesForCurrentGroup)
        {
            inGroupCooldown = false;
            StopCoroutine(groupCoroutine);
            SumEnemiesForCurrentGroup();
        }       
    }

    public virtual IEnumerator EnemyCooldown(float time)
    {
        inEnemyCooldown = true;
        yield return new WaitForSeconds(time);
        inEnemyCooldown = false;
    }
    public virtual IEnumerator GroupCooldown(float time)
    {
        inGroupCooldown = true;
        yield return new WaitForSeconds(time);
        inGroupCooldown = false;
        SumEnemiesForCurrentGroup();
    }

    public virtual IEnumerator WaveCooldown(float time)
    {
        inWaveCooldown = true;
        IncrementWaveCount();
        SumEnemiesPerWave();
        yield return new WaitForSeconds(time);
        enemiesForCurrentGroup = 0;
        SumEnemiesForCurrentGroup();
        inWaveCooldown = false;
    }
    
    public virtual IEnumerator GameStartCooldown(float time)
    {
        inGameStartCooldown = true;
        yield return new WaitForSeconds(time);
        gameStarted = true;
        IncrementWaveCount();
        SumEnemiesPerWave();
        SumEnemiesForCurrentGroup();
        inGameStartCooldown = false;
    }

    public void SumEnemiesPerWave()
    {
        
        for (int i = 0; i < waves[waveIterator].groups.Length; i++)
        {
            for (int j = 0; j < waves[waveIterator].groups[i].enemySpawn.Length; j++)
            {
                totalEnemiesPerWave += waves[waveIterator].groups[i].enemySpawn[j].GetAmountToSpawn();
                enemiesAlive = totalEnemiesPerWave;
            }
        } 
    }

    public void SumEnemiesForCurrentGroup()
    {
        if (groupIterator < waves[waveIterator].groups.Length)
        {
            for(int i = 0; i < waves[waveIterator].groups[groupIterator].enemySpawn.Length; i++)
            {
                enemiesForCurrentGroup += waves[waveIterator].groups[groupIterator].enemySpawn[i].GetAmountToSpawn();
            }
        }
    }

    void IncrementWaveCount()
    {
        currentWaves++;
    }

    public static void CountDeadEnemies()
    {
        instance.groupKillCount++;
        instance.enemiesAlive--;
    }
}
