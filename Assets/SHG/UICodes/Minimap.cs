using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Minimap : VisualElement, IUIComponent
{
  public Camera Camera;
  public const string CONTAINER_NAME = "minimap_container";
  RenderTexture renderTexture;
  public float ZoomLerp = 0.8f;

  public Minimap()
  {
    this.Init();
    this.CreateUI();
  }

  public void MoveCameraCenterTo(Vector3 pos)
  {
    this.Camera.transform.position = pos;
    this.Camera.transform.LookAt(pos);
  }

  public IEnumerator Zoom(float size, Action callback = null)
  {
    while (Mathf.Abs(this.Camera.orthographicSize - size) > float.Epsilon) {
      this.Camera.orthographicSize = Mathf.Lerp(
          this.Camera.orthographicSize,
          size,
          this.ZoomLerp * Time.deltaTime
          );
      yield return (null);
    }
    callback?.Invoke();
  }

  void Init()
  {
    this.renderTexture = Resources.Load<RenderTexture>("Textures/Minimap Render Texture");
  }

  void CreateUI()
  {
    this.name = Minimap.CONTAINER_NAME;
    this.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(this.renderTexture));
  }

}
