using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    // Maybe add a WaveInfo file for all these variables?
    public static bool inTestingScene;
    public static bool gameStarted;
    public static bool inWaveCooldown;
    public static bool inGameStartCooldown;
    public static int currentWave;
    
    public float timeBetweenSpawns;
    public float timeBetweenWaves;
    public float timeBeforeGameStarts;
    public int enemiesPerWave;
    public int enemyIncreasePerWave;
    // public int waves; Would implement if we want to have a set number of waves instead of infinite 
    
    private float timeSinceLastSpawn;
    private int killCount;
    private int enemiesToSpawn;

    // Would refactor to instead spawn from a list of object pools and set amount of enemies per pool

    void Awake()
    {
        if (inTestingScene)
        {
            StartCoroutine(GameStartCooldown());
            enemiesToSpawn = enemiesPerWave;
        }
    }

    void Update()
    {
        SpawnWave(enemiesPerWave);
        killCount = TestEnemy.deadTestEnemies;
    }


    // Might want to change this to be in a Game State Manager Class
    public static void OnSceneLoaded(string scene)
    {
        if (scene == "Testing")
        {
            inTestingScene = true;
        }
        else
        {
            Debug.LogWarning("Wrong Scene, current scene is: " + scene);
        }
    }

    // Might want to change WaveCooldown count to be in a Game State Manager Class,
    // then have the wave manager look at if it can spawn mobs or not
    void SpawnWave(int enemiesPerWave)
    {

        if (inTestingScene && enemiesToSpawn != 0 && gameStarted)
        {
            if (Time.time > timeSinceLastSpawn)
            {
                ObjectPooler.SpawnTestEnemy();
                timeSinceLastSpawn = Time.time + timeBetweenSpawns;
                enemiesToSpawn--;
            }
        }
        if (killCount == enemiesPerWave)
        {
            StartCoroutine(WaveCooldown());
        }
    }

    // Increasing enemies per wave, and resetting kill count and dead test enemies
    public virtual IEnumerator WaveCooldown()
    {
        enemiesPerWave += enemyIncreasePerWave;
        inWaveCooldown = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        ResetPerWave();
        IncrementWaveCount();
        inWaveCooldown = false;
    }

    
    public virtual IEnumerator GameStartCooldown()
    {
        inGameStartCooldown = true;
        yield return new WaitForSeconds(timeBeforeGameStarts);
        gameStarted = true;
        IncrementWaveCount();
        inGameStartCooldown = false;
    }

    void ResetPerWave()
    {
        enemiesToSpawn = enemiesPerWave;
        TestEnemy.ResetDeadTestEnemies();
    }

    void IncrementWaveCount()
    {
        currentWave++;
    }
}
