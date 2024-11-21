using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] Transform[] _npcSpawnLocations;
    public Transform[] NPCSpawnLocations => _npcSpawnLocations;

    void Start()
    {
        if (WaveManager.instance is null) return;
        WaveManager.instance.gameStarting += WaveManager_GameStarted;
        WaveManager.instance.waveStarted += WaveManager_WaveStarted;
    }

    private void OnDestroy()
    {
        WaveManager.instance.gameStarting -= WaveManager_GameStarted;
        WaveManager.instance.waveStarted -= WaveManager_WaveStarted;
    }

    private void WaveManager_GameStarted(object sender, GameStartedEventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void WaveManager_WaveEnded(object sender, WaveEndedEventArgs e)
    {
        gameObject.SetActive(true);
    }

}