using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class TitleSceneUI : MonoBehaviour{

  const string CONTAINER_NAME = "title-container";
  VisualElement root;

  void Awake() {
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.name = TitleSceneUI.CONTAINER_NAME;
    root.style.height = Length.Percent(100);
    root.style.width = Length.Percent(100);
  }

  void Start() {
    this.CreateUI();
  }

  public void CreateUI() {
    Button startButton = new Button();
    startButton.name = "startButton";
    startButton.RegisterCallback<ClickEvent>(this.OnClickStartButton);

    Label startButtonLabel = new Label("Start Game");
    startButton.Add(startButtonLabel);
    this.root.Add(startButton);

    Button endButton = new Button();
    endButton.name = "endButton";
    endButton.RegisterCallback<ClickEvent>(this.OnClickEndButton);   
    Label endButtonLabel = new Label("End Game");
    endButton.Add(endButtonLabel);
    root.Add(endButton);
  }

  void OnClickStartButton(ClickEvent e) {
    GameSceneManager.Shared.StartLoadScene(GameSceneManager.SceneName.CombatScene);
  }

  void OnClickEndButton(ClickEvent e) {
    Application.Quit();
  }
}
