using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class TitleSceneUI : MonoBehaviour{

  public const string CONTAINER_NAME = "title-container";
  public const string CONTAINER_CLASS = "splash-container";
  VisualElement root;

  void Awake() {
    this.root = this.GetComponent<UIDocument>().rootVisualElement;
    this.root.name = TitleSceneUI.CONTAINER_NAME;
    this.root.AddToClassList(TitleSceneUI.CONTAINER_CLASS);
    root.style.height = Length.Percent(100);
    root.style.width = Length.Percent(100);
  }

  void Start() {
    this.CreateUI();
    AudioManager.Shared.ChangeBgmTo(AudioManager.TITLE_MUSIC);
  }

  public void CreateUI() {
    Button startButton = new Button();
    startButton.name = "start-button";
    startButton.RegisterCallback<ClickEvent>(this.OnClickStartButton);

    Label startButtonLabel = new Label("Start Game");
    startButton.Add(startButtonLabel);
    this.root.Add(startButton);

    Button endButton = new Button();
    endButton.name = "end-button";
    endButton.RegisterCallback<ClickEvent>(this.OnClickEndButton);   
    Label endButtonLabel = new Label("End Game");
    endButton.Add(endButtonLabel);
    root.Add(endButton);
  }

  void OnClickStartButton(ClickEvent e) {
    GameSceneManager.Shared.StartLoadScene(GameSceneManager.SceneName.CombatScene);
  }

  void OnClickEndButton(ClickEvent e) {
    GameManager.Shared.EndGame();
  }
}
