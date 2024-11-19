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
    public GameObject northAndSouthFog;

    private Animator eastFogAnimator;
    private Animator westFogAnimator;
    private Animator northAndSouthFogAnimator;

    private Transform[] shopLocations;
    private int waveCounter = 1;
    private float boundsIncrement = 75;
    private int plainsLevelWaveStart;
    private int goblinsLevelWaveStart;

    void Start()
    {
        westFogAnimator = westFog.GetComponent<Animator>();
        eastFogAnimator = eastFog.GetComponent<Animator>();
        northAndSouthFogAnimator = northAndSouthFog.GetComponent<Animator>();

        if (WaveManager.instance is null || plainsBoundary is null || goblinBoundary is null) return;
        WaveManager.instance.waveFinished += WaveManager_WaveFinished;
        WaveManager.instance.waveStarted += WaveManager_WaveStarted;
        plainsLevelWaveStart = WaveManager.instance.startPlainsLevel;
        goblinsLevelWaveStart = WaveManager.instance.startGoblinLevel;

        PhysicalShop physicalShop = shop.GetComponent<PhysicalShop>();
        if (physicalShop != null)
        {
            shopLocations = physicalShop.ShopSpawnLocations;
        }

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
                northAndSouthFogAnimator.SetTrigger("TriggerSecondLevel");
                westFogAnimator.SetTrigger("TriggerSecondLevel");
            }
            else
            {
                goblinBoundary.SetActive(true);
                westFogAnimator.SetTrigger("TriggerThirdLevel");
            }
            xMinBoundary += boundsIncrement;
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
                if (shopLocations.Length > 0)
                {
                    shop.transform.position = shopLocations[0].position;
                }
                eastFogAnimator.SetTrigger("TriggerSecondLevel");
            }
            else
            {
                goblinBoundary.SetActive(false);
                if (shopLocations.Length > 1)
                {
                    shop.transform.position = shopLocations[1].position;
                }
                eastFogAnimator.SetTrigger("TriggerThirdLevel");
            }
            xMaxBoundary += boundsIncrement;
            BoundsUpdated();
        }
    }

    private void BoundsUpdated()
    {
        UpdateBoundary?.Invoke();
    }
}
