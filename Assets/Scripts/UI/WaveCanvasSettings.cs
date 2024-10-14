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
<<<<<<< HEAD
        if (!WaveManager.instance.isGameFinished)
        {
            if (WaveManager.instance.inPlaceholderScene && WaveManager.instance.gameStarted)
            {
                currentWaveText.gameObject.SetActive(true);
                currentWaveText.text = "Wave: " + WaveManager.instance.currentWaves;
                totalEnemiesPerWaveText.gameObject.SetActive(true);
                totalEnemiesPerWaveText.text = "Enemies in Wave: " + WaveManager.instance.enemiesAlive + "/" + WaveManager.instance.totalEnemiesPerWave;
            }

            if (WaveManager.instance.inGameStartCooldown)
=======
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
>>>>>>> b1a041f37349cc6856af0ad78da76a10ba364289
            {
                waveCountdownText.gameObject.SetActive(true);
                gameStartCountdownTime -= 1 * Time.deltaTime;
                waveCountdownText.text = "Time Until Wave Starts: " + gameStartCountdownTime.ToString("0") + " seconds";
            }
<<<<<<< HEAD
            else if (WaveManager.instance.inWaveCooldown)
=======
            else if (WaveManager.inWaveCooldown)
>>>>>>> b1a041f37349cc6856af0ad78da76a10ba364289
            {
                waveCountdownTime -= 1 * Time.deltaTime;
                waveCountdownText.text = "Time Until Next Wave: " + waveCountdownTime.ToString("0") + " seconds";
                waveCountdownText.gameObject.SetActive(true);
            }
            else
            {
<<<<<<< HEAD
                waveCountdownTime = WaveManager.instance.timeBetweenWaves;
=======
                waveCountdownTime = WaveManager.timeBetweenWaves;
>>>>>>> b1a041f37349cc6856af0ad78da76a10ba364289
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
