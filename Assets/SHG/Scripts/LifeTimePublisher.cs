using System;
using UnityEngine;

public class LifeTimePublisher : MonoBehaviour
{
  public Action AfterDestroyed;
  public Action AfterDisabled;

  void OnDisable()
  {
    if (this.AfterDisabled!= null) {
      this.AfterDisabled.Invoke();
    }
  }

  void OnDestroy()
  {
    if (this.AfterDestroyed != null) {
      this.AfterDestroyed.Invoke();
    }
  }
}
