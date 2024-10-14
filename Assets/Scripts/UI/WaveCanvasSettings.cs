using UnityEngine;
using TMPro;

public class WaveCanvasSettings : MonoBehaviour
{

    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI waveCountdownText;
    public TextMeshProUGUI totalEnemiesPerWaveText;
    public TextMeshProUGUI winText;
    public WaveManager waveManager;

    private float waveCountdownTime;
    private float gameStartCountdownTime;

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        gameStartCountdownTime = waveManager.timeBeforeGameStarts;
    }

    void Update()
    {
        if (!WaveManager.isGameFinished)
        {
            if (WaveManager.inPlaceholderScene && WaveManager.gameStarted)
            {
                currentWaveText.gameObject.SetActive(true);
                currentWaveText.text = "Wave: " + WaveManager.currentWaves;
                totalEnemiesPerWaveText.gameObject.SetActive(true);
                totalEnemiesPerWaveText.text = "Enemies in Wave: " + WaveManager.enemiesAlive + "/" + WaveManager.totalEnemiesPerWave;
            }

            if (WaveManager.inGameStartCooldown)
            {
                waveCountdownText.gameObject.SetActive(true);
                gameStartCountdownTime -= 1 * Time.deltaTime;
                waveCountdownText.text = "Time Until Wave Starts: " + gameStartCountdownTime.ToString("0") + " seconds";
            }
            else if (WaveManager.inWaveCooldown)
            {
                waveCountdownTime -= 1 * Time.deltaTime;
                waveCountdownText.text = "Time Until Next Wave: " + waveCountdownTime.ToString("0") + " seconds";
                waveCountdownText.gameObject.SetActive(true);
            }
            else
            {
                waveCountdownTime = WaveManager.timeBetweenWaves;
                waveCountdownText.gameObject.SetActive(false);
            }
        }
        else
        {
            winText.text = "You Win!";
            winText.gameObject.SetActive(true);
            waveCountdownText.gameObject.SetActive(false);
            totalEnemiesPerWaveText.gameObject.SetActive(false);
            currentWaveText.gameObject.SetActive(false);
        }
    }
}
