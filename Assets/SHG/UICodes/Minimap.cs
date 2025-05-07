using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Minimap : VisualElement
{
  public const string FLAG_ICON = "flag";
  public const string PLAYER_ICON = "blue_circle";
  public const string DOOR_ICON = "door";
  public const string CONTAINER_NAME = "minimap_container";
  public const float DEFAULT_ZOOM = 20f;
  public Camera Camera {
    get => this.camera;
    set {
      this.camera = value;
      if (value != null) {
        this.SetCamera();
      }
    }
  }
  public float ZoomLerp = 0.8f;
  public Sprite FlagIcon { get; private set; }
  public Sprite PlayerIcon { get; private set; }
  public Sprite DoorIcon { get; private set; }
  RenderTexture renderTexture;
  Camera camera;

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
    while (this.Camera != null &&
        Mathf.Abs(this.Camera.orthographicSize - size) > float.Epsilon) {
      this.Camera.orthographicSize = Mathf.Lerp(
          this.Camera.orthographicSize,
          size,
          this.ZoomLerp * Time.deltaTime
          );
      yield return (null);
    }
    callback?.Invoke();
  }

  public GameObject AddMinimapIconTo(GameObject obj, Sprite sprite, float scale = 1)
  {
    var minimapIcon = new GameObject();
    minimapIcon.name = "Minimap Icon";
    minimapIcon.layer = LayerMask.NameToLayer("UIRenderOnly");
    minimapIcon.transform.parent = obj.transform;
    minimapIcon.transform.localPosition = new (0, MainCamera.CAMERA_HEIGHT, 0);
    minimapIcon.transform.Rotate(new (90, 0, 0));
    minimapIcon.transform.localScale = new (scale, scale, 0);
    var renderer = minimapIcon.AddComponent<SpriteRenderer>();
    renderer.sprite = sprite;
    return (minimapIcon); 
  }

  void Init()
  {
    this.renderTexture = Resources.Load<RenderTexture>("Textures/Minimap Render Texture");
    this.FlagIcon = Resources.Load<Sprite>($"Sprites/{FLAG_ICON}");
    this.PlayerIcon = Resources.Load<Sprite>($"Sprites/{PLAYER_ICON}");
    this.DoorIcon = Resources.Load<Sprite>($"Sprites/{DOOR_ICON}");
  }

  void CreateUI()
  {
    this.name = Minimap.CONTAINER_NAME;
    this.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(this.renderTexture));
  }

  void SetCamera()
  {
  }

}
