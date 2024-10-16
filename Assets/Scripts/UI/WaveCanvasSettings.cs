using UnityEngine;
using TMPro;
using System.Collections;

public class WaveCanvasSettings : MonoBehaviour
{

    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI waveCountdownText;
    public TextMeshProUGUI totalEnemiesPerWaveText;
    public TextMeshProUGUI winText;

    void Start()
    {
        if (WaveManager.instance is null) return;

		WaveManager.instance.gameStarting += WaveManager_GameStarted;
		WaveManager.instance.waveStarted += WaveManager_WaveStarted;
		WaveManager.instance.waveFinished += WaveManager_WaveFinished;
		WaveManager.instance.enemyDied += WaveManager_EnemyDied;
		WaveManager.instance.StartGame();
    }

	private void OnDestroy()
	{
		WaveManager.instance.gameStarting -= WaveManager_GameStarted;
		WaveManager.instance.waveStarted -= WaveManager_WaveStarted;
		WaveManager.instance.waveFinished -= WaveManager_WaveFinished;
		WaveManager.instance.enemyDied -= WaveManager_EnemyDied;
	}

	private void WaveManager_GameStarted(object sender, GameStartedEventArgs e)
	{
		StartCoroutine(StartGameCountDown(e.countDown));
	}

	private void WaveManager_WaveStarted(object sender, WaveStartedEventArgs e)
	{
		totalEnemiesPerWaveText.gameObject.SetActive(true);
		currentWaveText.gameObject.SetActive(true);

		currentMaxEnemyCount = e.enemyCount;
		UpdateEnemyText(currentMaxEnemyCount);
		currentWaveText.text = $"Wave {e.waveNum}";
	}

	private void WaveManager_WaveFinished(object sender, WaveEndedEventArgs e)
	{
		StartCoroutine(SetCountDownText(e.timeTillNextWave));
	}

	private void WaveManager_EnemyDied(object sender, EnemyDiedEventArgs e)
	{
		UpdateEnemyText(e.remainingEnemies);
	}

	private void UpdateEnemyText(int count)
	{
		totalEnemiesPerWaveText.text = $"Enemies {count}/{currentMaxEnemyCount}";
	}

	IEnumerator StartGameCountDown(int start)
	{
		yield return SetCountDownText(start);
		WaveManager.instance.SpawnWaves();
	}

	IEnumerator SetCountDownText(int start)
	{
		var count = start;
		waveCountdownText.gameObject.SetActive(true);
		totalEnemiesPerWaveText.gameObject.SetActive(false);
		currentWaveText.gameObject.SetActive(false);
		while (count > 0)
		{
			waveCountdownText.text = count.ToString();
			yield return new WaitForSeconds(1);
			count--;
		}
		waveCountdownText.gameObject.SetActive(false);
	}

	int currentMaxEnemyCount;
}
