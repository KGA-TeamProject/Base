using System;
using System.Collections;
using UnityEngine;

public class GameSceneManager : Singleton<GameSceneManager>
{
  public enum SceneName
  {
    FirstScene
  }

  public event Action<SceneName> OnSceneLoaded;

  public SceneName CurrentScene { get; private set; }

  public void StartLoadScene(SceneName scene) 
  {
    this.StartCoroutine(this.CreateLoadSceneRoutine(scene));
  }

  IEnumerator CreateLoadSceneRoutine(SceneName scene)
  {
    AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Enum.GetName(typeof(SceneName), scene));
    load.allowSceneActivation = true;
    while (!load.isDone) {
      yield return (null);
    }
    if (this.OnSceneLoaded != null) {
      this.OnSceneLoaded.Invoke(scene);
    }
  }
} 
