using TMPro;
using UnityEngine;

public class SetTargetFrameRate : MonoBehaviour
{
    public int targetFrameRate = 30;
    public bool overrideFps = true;
    public bool vSyncEnabled = true;
    public TextMeshProUGUI fpsCounter;

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
        if (overrideFps)
            Application.targetFrameRate = targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (overrideFps && Application.targetFrameRate != targetFrameRate)
            Application.targetFrameRate = targetFrameRate;

        UIDebugUtility.instance.UpdateFps(1 / Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            Time.timeScale = Mathf.Max(0.1f, Time.timeScale - 0.1f);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Time.timeScale = Mathf.Min(1.0f, Time.timeScale + 0.1f);
	}
}
