using System;
using System.Collections;
using UnityEngine;

public class Collectible : MonoBehaviour
{
 
  const float ROTATION_STEP = 120f;
  const float FLY_STEP = 5f;
  const float FLY_THRESHOLD = 0.5f;
  public Action OnCollected;
  Coroutine rotateRoutine;
  Coroutine flyRoutine;

  void OnEnable()
  {
    if (this.rotateRoutine == null) {
      this.rotateRoutine = this.StartCoroutine(this.Rotate());
    }
  }

  void OnDisable()
  {
    if (this.rotateRoutine != null) {
      this.StopCoroutine(this.rotateRoutine);
      this.rotateRoutine = null;
    }
    if (this.flyRoutine != null) {
      this.StopCoroutine(this.flyRoutine);
      this.flyRoutine = null;
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  IEnumerator Rotate()
  {
    while (true) {
      this.transform.Rotate(
          new (0, Time.deltaTime * Collectible.ROTATION_STEP, 0));
      yield return (null);
    }
  }

  void OnTriggerEnter(Collider collider) 
  {
    if (collider.tag == "Player") {
      this.CollectedBy(collider.transform);
    }
  }

  void CollectedBy(Transform collector)
  {
    this.flyRoutine = this.StartCoroutine(
      this.FlyTo(collector, () => {
        this.flyRoutine = null;  
        AudioManager.Shared.PlayEffect(AudioManager.Shared.CoinEffect);
        if (this.OnCollected != null) {
          OnCollected.Invoke();
        }
        this.gameObject.SetActive(false);
      }
    ));
  }

  IEnumerator FlyTo(Transform dest, Action callback)
  {
    while (Vector3.Distance(
          this.transform.position, dest.position) > Collectible.FLY_THRESHOLD) {
      this.transform.position = Vector3.Lerp(
          this.transform.position,
          dest.position, 
          Time.deltaTime * Collectible.FLY_STEP
          );
      yield return (null);
    }

    if (callback != null) {
      callback.Invoke();
    }
  }
}
