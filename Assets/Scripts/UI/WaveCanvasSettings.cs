using UnityEngine;
using TMPro;

public class WaveCanvasSettings : MonoBehaviour
{

    public TextMeshProUGUI currentWaveText;
    public TextMeshProUGUI waveCountdownText;
    public WaveManager waveManager;

    private float waveCountdownTime;
    private float waveCountdownTimeHolder;
    private float gameStartCountdownTime;

    // Setting countdown for first wave to spawn only once
    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        if (waveManager is object)
            gameStartCountdownTime = waveManager.timeBeforeGameStarts;
 
    }

    // Allows to change wave countdown text during the game
    void Update()
    {
        if (waveManager is null)
            return;

        if (WaveManager.inTestingScene && WaveManager.gameStarted)
        {
            currentWaveText.text = "Wave: " + WaveManager.currentWave;
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
            waveCountdownTime = waveManager.timeBetweenWaves;
            waveCountdownText.gameObject.SetActive(false);
        }
    }


}
