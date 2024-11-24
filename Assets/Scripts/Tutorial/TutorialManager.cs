using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    private void Start()
    {
        LevelLoadManager.instance.LoadSceneAsync(sceneToLoad);
    }
}
