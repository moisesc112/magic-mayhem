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


    public GameObject shop;
    public GameObject plainsBoundary;
    public GameObject goblinBoundary;
    public GameObject westFog;
    public GameObject eastFog;
    public GameObject northFog;
    public GameObject southFog;
    

    public Vector3 shopPlains;
    public Vector3 shopGoblins;

    private Animator westFogAnimator;
    private int waveCounter = 1;
    public int plainsLevelWaveStart;
    public int goblinsLevelWaveStart;

    void Start()
    {
        westFogAnimator = westFog.GetComponent<Animator>();
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
        if (waveCounter == plainsLevelWaveStart || waveCounter == goblinsLevelWaveStart)
        {
            if (waveCounter == plainsLevelWaveStart)
            {
                plainsBoundary.SetActive(true);
                eastFog.transform.position = new Vector3(eastFog.transform.position.x + 74.07f, eastFog.transform.position.y, eastFog.transform.position.z);
            }
            else
            {
                goblinBoundary.SetActive(true);
                eastFog.transform.position = new Vector3(eastFog.transform.position.x + 74.07f, eastFog.transform.position.y, eastFog.transform.position.z);
            }
             xMinBoundary += 75;
            BoundsUpdated();
        }
    }

    private void WaveManager_WaveFinished(object sender, WaveEndedEventArgs e)
    {
        waveCounter++;
        if (waveCounter == plainsLevelWaveStart || waveCounter == goblinsLevelWaveStart)
        {
            if (waveCounter == plainsLevelWaveStart)
            {
                plainsBoundary.SetActive(false);
                shop.transform.position = shopPlains;
                westFogAnimator.SetTrigger("TriggerSecondLevel");
            }
            else
            {
                goblinBoundary.SetActive(false);
                shop.transform.position = shopGoblins;
                northFog.transform.position = new Vector3(northFog.transform.position.x + 50, northFog.transform.position.y, northFog.transform.position.z);
                southFog.transform.position = new Vector3(southFog.transform.position.x + 50, southFog.transform.position.y, southFog.transform.position.z);
                westFogAnimator.SetTrigger("TriggerThirdLevel");
            }
            xMaxBoundary += 75;
            BoundsUpdated();
        }
    }

    private void BoundsUpdated()
    {
        UpdateBoundary?.Invoke();
    }
}
