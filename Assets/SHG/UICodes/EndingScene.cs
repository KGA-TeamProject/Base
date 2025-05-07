using UnityEngine;
using UnityEngine.UIElements;

public class EndingScene : MonoBehaviour
{
  const string CONTAINER_NAME = "ending-container";
  const string CONTAINER_CLASS = TitleSceneUI.CONTAINER_CLASS;
  VisualElement root;

  void Awake()
  {
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.name = EndingScene.CONTAINER_NAME;
    this.root.AddToClassList(EndingScene.CONTAINER_CLASS);
  }

  void Start()
  {
    AudioManager.Shared.ChangeBgmTo(AudioManager.ENDING_MUSIC);
    this.CreateUI();
  }

  void CreateUI()
  {
    var label = new Label();
    label.AddToClassList("main-label");
    label.text = "Thank you for playing!";
    this.root.Add(label);
    var button = new Button();
    button.name = "end-button";
    var buttonLabel = new Label("End Game");
    button.Add(buttonLabel);
    button.RegisterCallback<ClickEvent>((e) => {
      GameManager.Shared.EndGame();
    });
    this.root.Add(button);
  }
}
