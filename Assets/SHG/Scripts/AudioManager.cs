using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
  public const string TITLE_MUSIC = "CGM4_Fairy_Harp_Melody_Loop";
  public const string ENDING_MUSIC = "CGM4_Lose_Jingle_Loop";
  public const string COIN_SOUND = "CGM4_Button_Select_02";
  public const string CLEAR_SOUND = "CGM4_Mission_Success_Fast";
  const float BGM_VOLUME = 0.4f;
  const float EFFECT_VOLUME = 0.7f;
  const float BGM_FADE_STEP = 0.005f;
  public AudioClip CoinEffect { get; private set; }
  public AudioClip ClearEffect { get; private set; }
  AudioClip backgroundClip;
  AudioSource backgroundSource;
  AudioSource foregroundSource;
  Coroutine bgmFadeRoutine;

  public void ChangeBgmTo(string name, string path = null)
  {
    AudioClip clip = Resources.Load<AudioClip>(path ?? $"Audio/{name}");
    if (backgroundSource.isPlaying) {
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
    else {
      this.backgroundSource.clip = clip;
      this.backgroundSource.volume = 0;
      this.backgroundSource.Play();
      this.bgmFadeRoutine = this.StartCoroutine(
          this.FadeInBgm(AudioManager.BGM_VOLUME, 
            () => this.bgmFadeRoutine = null)
          );
    }
  }

  public void PlayEffect(AudioClip clip)
  {
    this.foregroundSource.clip = clip;
    this.foregroundSource.Play();
  }

  void Awake()
  {
    this.backgroundSource = this.gameObject.AddComponent<AudioSource>();
    this.backgroundSource.loop = true;
    this.backgroundSource.volume = AudioManager.BGM_VOLUME;
    this.foregroundSource = this.gameObject.AddComponent<AudioSource>();
    this.foregroundSource.loop = false;
    this.foregroundSource.volume = AudioManager.EFFECT_VOLUME;
    this.CoinEffect = Resources.Load<AudioClip>($"Audio/{AudioManager.COIN_SOUND}");
    this.ClearEffect = Resources.Load<AudioClip>($"Audio/{AudioManager.CLEAR_SOUND}");
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
