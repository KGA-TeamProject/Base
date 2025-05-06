using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
  const string TITLE_MUSIC = "CGM4_Fairy_Harp_Melody_Loop";
  const float BGM_VOLUME = 0.5f;
  const float BGM_FADE_STEP = 0.005f;
  AudioClip backgroundClip;
  AudioSource backgroundSource;
  Coroutine bgmFadeRoutine;

  public void ChangeBgmTo(string name, string path = null)
  {
    AudioClip clip = Resources.Load<AudioClip>(path ?? $"Audio/{name}");
    this.bgmFadeRoutine = this.StartCoroutine(
      this.FadeOutBgm(() => {
        this.backgroundSource.Stop();
        this.backgroundSource.clip = clip;
        this.backgroundSource.Play();
        this.bgmFadeRoutine = this.StartCoroutine(
          this.FadeInBgm(AudioManager.BGM_VOLUME, 
            () => this.bgmFadeRoutine = null)
          );
        })
      );
  }

  void Awake()
  {
    this.backgroundSource = this.gameObject.AddComponent<AudioSource>();
    this.backgroundSource.loop = true;
    this.backgroundSource.volume = AudioManager.BGM_VOLUME;
    this.backgroundClip = Resources.Load<AudioClip>($"Audio/{AudioManager.TITLE_MUSIC}");

  }
  // Start is called before the first frame update
  void Start()
  {
    this.PlayBgm();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void PlayBgm()
  {
    this.backgroundSource.clip = this.backgroundClip;
    this.backgroundSource.Play();
  }

  IEnumerator FadeOutBgm(Action callback = null)
  {
    while (backgroundSource.volume > float.Epsilon) {
      this.backgroundSource.volume -= AudioManager.BGM_FADE_STEP;
      yield return (null);
    }
    if (callback != null) {
      callback.Invoke();
    }
  }
  IEnumerator FadeInBgm(float volume, Action callback = null)
  {
    while (backgroundSource.volume < volume) {
      this.backgroundSource.volume += AudioManager.BGM_FADE_STEP;
      yield return (null);
    }
    if (callback != null) {
      callback.Invoke();
    }
  }
}
