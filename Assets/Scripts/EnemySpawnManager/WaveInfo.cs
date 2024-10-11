using UnityEngine;
public class WaveInfo : MonoBehaviour
{
    [System.Serializable]
    public class WaveContent
    {
        [System.Serializable]
        public class GroupContent
        {
            [System.Serializable]
            public class EnemyContent
            {
                public enum EnemyToSpawn
                {
                    GoblinEnemy,
                    GolemEnemy,
                    TestEnemy,
                    WarChiefEnemy
                };

                [SerializeField] public EnemyToSpawn enemyToSpawn;
                [SerializeField] public int amountToSpawn;

                public int GetAmountToSpawn()
                {
                    return amountToSpawn;
                }
            }
            [SerializeField] public EnemyContent[] enemySpawn;
            [SerializeField] public float timeToNextGroup;

            public EnemyContent[] GetEnemySpawn()
            {
                return enemySpawn;
            }
            public float GetTimeToNextGroup()
            {
                return timeToNextGroup;
            }
        }
        [SerializeField] public GroupContent[] groups;
        [SerializeField] public float timeToNextWave;
        [SerializeField] public float timeBetweenEnemies;


        public GroupContent[] GetGroups()
        {
            return groups;
        }
        public float GetTimeToNextWave()
        {
            return timeToNextWave;
        }
        public float GetTimeBetweenEnemies()
        {
            return timeBetweenEnemies;
        }

    }

    [SerializeField] WaveContent[] waves;

    public WaveContent[] GetWaveContents()
    {
        return waves;
    }
    
}
