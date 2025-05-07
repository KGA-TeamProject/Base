using System;
using System.Collections;
using UnityEngine;
public class GameSceneManager : Singleton<GameSceneManager>
{

  AudioListener titleAudioListener;

  public enum SceneName
  {
    TitleScene,
    CombatScene,
    EndingScene
  }

  public event Action<SceneName> OnSceneLoaded;

  public SceneName CurrentScene { get; private set; }


  public void StartLoadScene(SceneName scene) 
  {
    this.StartCoroutine(this.CreateLoadSceneRoutine(scene));
  }

  IEnumerator CreateLoadSceneRoutine(SceneName scene)
  {
    this.CurrentScene = scene;
    AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Enum.GetName(typeof(SceneName), scene));
    load.allowSceneActivation = true;
    while (!load.isDone) {
      yield return (null);
    }
    this.OnGameSceneLoaded();
  }

  void OnGameSceneLoaded()
  {
    if (this.CurrentScene == SceneName.TitleScene) {
    }
    else if (this.CurrentScene == SceneName.CombatScene) {
      UIManager.Shared.loadingUI.Show();
      this.SetCamera();
    }
    if (this.OnSceneLoaded != null) {
      this.OnSceneLoaded.Invoke(this.CurrentScene);
    }
  }

  void SetCamera() {
    if (Camera.main != null) {
      Camera.main.gameObject.AddComponent<MainCamera>();
      Camera.main.fieldOfView = 50f;
    }
  }
} 
