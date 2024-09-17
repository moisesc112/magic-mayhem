using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ObjectPooler : MonoBehaviour
{
    // make a list of gameobjects to spawn multiple enemy types in same script
    public static ObjectPool<TestEnemy> testEnemyPool;
    
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform parent;
    [SerializeField] private TestEnemy testEnemyPreFab;
    [SerializeField] private int maxPoolSize;

    // https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html
    private void Start()
    {
        testEnemyPool = new ObjectPool<TestEnemy>(CreateTestEnemy, OnTakeTestEnemyFromPool, OnReturnTestEnemyToPool, OnDestroyTestEnemy, true, 1, maxPoolSize);
    }

    // Putting all test enemies under the same parent in the hierachry to look clean
    // Giving all test enemies same tag to be registered in HandleDeath() in Health Component
    private TestEnemy CreateTestEnemy()
    {
        TestEnemy testEnemy = Instantiate(testEnemyPreFab, parent);
        testEnemy.gameObject.tag = "TestEnemyPool";
        testEnemy.SetPool(testEnemyPool);
        return testEnemy;
    }

    // When taken from pool, test enemy spawns at one of spawnPoints set in the inspector
    // Maybe move spawnPoint logic to WaveManager.cs
    private void OnTakeTestEnemyFromPool(TestEnemy testEnemy)
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        testEnemy.transform.position = randomSpawnPoint.position;
        testEnemy.gameObject.SetActive(true);
    }

    private void OnReturnTestEnemyToPool(TestEnemy testEnemy)
    {
        testEnemy.gameObject.SetActive(false);
    }

    private void OnDestroyTestEnemy(TestEnemy testEnemy)
    {
        Destroy(testEnemy);
    }

    // This is how an enemy gets spawned in the wave, used in WaveManager.cs
    public static void SpawnTestEnemy()
    {
        testEnemyPool.Get();
    }

}
