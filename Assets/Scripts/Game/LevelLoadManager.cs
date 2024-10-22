using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadManager : MonoBehaviour
{
	public static LevelLoadManager instance => _instance;
	public event EventHandler<LevelLoadedArgs> sceneLoaded;
	
	void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this);
			return;
		}

		_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void LoadSceneAsync(string sceneName)
	{
		StopCoroutine(nameof(LoadSceneAsync));
		StartCoroutine(DoLoadSceneAsync(sceneName));
	}

	private IEnumerator DoLoadSceneAsync(string sceneName)
	{
		_load = SceneManager.LoadSceneAsync(sceneName);
		_load.allowSceneActivation = false;
		while (!_load.isDone)
		{
			// Scenes will only load 90% async
			if (_load.progress >= 0.9f)
			{
				break;
			}
			Debug.Log(_load.progress);
			yield return null;
		}
		// Uncomment line below to simulate long load times.
		//yield return new WaitForSeconds(10.0f);
		RaiseSceneLoaded(sceneName);
	}

	public void ActivateLoadedScene()
	{
		if (_load is null) return;

		_load.allowSceneActivation = true;
	}

	private void RaiseSceneLoaded(string sceneName)
	{
		instance.sceneLoaded?.Invoke(instance, new LevelLoadedArgs(sceneName));
	}

	AsyncOperation _load;
	static LevelLoadManager _instance;
}

public sealed class LevelLoadedArgs : EventArgs
{
    public LevelLoadedArgs(string scene)
    {
        sceneName = scene;
    }
    string sceneName { get; }
}
