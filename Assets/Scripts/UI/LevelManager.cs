using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class LevelManager : Singleton<LevelManager>
{
	public float xMinBoundary;
    public float xMaxBoundary;
    public float zMinBoundary;
    public float zMaxBoundary;
    public event System.Action UpdateBoundary;

    public GameObject plainsBoundary;
    public GameObject goblinBoundary;


    private int waveCounter = 1;
    public int plainsLevelWaveStart;
    public int goblinsLevelWaveStart;

    void Start()
    {
        if (WaveManager.instance is null || plainsBoundary is null || goblinBoundary is null) return;
        WaveManager.instance.waveFinished += WaveManager_WaveFinished;
        WaveManager.instance.waveStarted += WaveManager_WaveStarted;
    }

    private void OnDestroy()
    {
        if (WaveManager.instance is null) return;
        WaveManager.instance.waveFinished -= WaveManager_WaveFinished;
        WaveManager.instance.waveStarted -= WaveManager_WaveStarted;
    }

    private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
    {
        if (waveCounter == plainsLevelWaveStart)
        {
            plainsBoundary.SetActive(true);
            xMinBoundary += 75;
            BoundsUpdated();
        }
        else if (waveCounter == goblinsLevelWaveStart)
        {
            goblinBoundary.SetActive(true);
            xMinBoundary += 75;
            BoundsUpdated();
        }
    }

    private void WaveManager_WaveFinished(object sender, WaveEndedEventArgs e)
    {
        waveCounter++;
        if (waveCounter == plainsLevelWaveStart)
        {
            plainsBoundary.SetActive(false);
            xMaxBoundary += 75;
            BoundsUpdated();
        }
        else if (waveCounter == goblinsLevelWaveStart)
        {
            goblinBoundary.SetActive(false);
            xMaxBoundary += 75;
            BoundsUpdated();
        }
    }

    private void BoundsUpdated()
    {
        UpdateBoundary?.Invoke();
    }
}
