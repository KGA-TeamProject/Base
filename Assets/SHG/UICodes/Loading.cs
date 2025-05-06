using UnityEngine;
using UnityEngine.UIElements;

public class LoadingUI : MonoBehaviour
{
  public const string PREFAB_NAME = "LoadingUI";
  const string CONTAINER_NAME = "loading-container";
  const string MAIN_LABEL = "loading-label";
  VisualElement root;

  public void Show()
  {
    this.root.visible = true;
  }

  public void Hide()
  {
    this.root.visible = false;
  }

  void Awake()
  {
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.name = LoadingUI.CONTAINER_NAME;
    this.root.style.height = Length.Percent(100);
    this.root.style.width = Length.Percent(100);
  }

  void Start()
  {
    this.CreateUI();    
  }

  // Update is called once per frame
  void Update()
  {


  }

  void CreateUI()
  {
    var label = new Label("Now Loading...");
    label.name = LoadingUI.MAIN_LABEL;  
    this.root.Add(label);
  }
}
