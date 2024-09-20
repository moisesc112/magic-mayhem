using UnityEngine;
using UnityEngine.Pool;
public class TestEnemy : MonoBehaviour
{
    public static int deadTestEnemies;

    private ObjectPool<TestEnemy> testEnemyPool;
    private TestEnemy testEnemy;

    public void SetPool(ObjectPool<TestEnemy> pool)
    {
        testEnemyPool = pool;
    }

    //Used to reset the amount of dead test enemies tracked during each wave
    public static void ResetDeadTestEnemies()
    {
        deadTestEnemies = 0;
    }

    //Used to count the amount of dead test enemies tracked during each wave
    public static void CountDeadTestEnemies()
    {
        deadTestEnemies++;
    }

    public void TestEnemyPoolRelease()
    {
        testEnemyPool.Release(this);
    }
}
