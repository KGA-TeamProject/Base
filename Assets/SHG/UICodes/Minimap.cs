using UnityEngine;
using UnityEngine.UIElements;

public class Minimap : VisualElement, IUIComponent
{
  public const string CONTAINER_NAME = "minimap_container";
  RenderTexture renderTexture;

  public Minimap()
  {
    this.Init();
    this.CreateUI();
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
