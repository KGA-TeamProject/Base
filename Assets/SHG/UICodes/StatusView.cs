using UnityEngine.UIElements;
using UnityEngine;

public class StatusView : VisualElement
{
  public const string CONTAINER_NAME = "statusview-container";
  public const string SECTION_CLASS = "statusview-section";
  public const string SECTION_ICON_CLASS = "statusview-section-icon";
  public const string SECTION_TEXT_CLASS = "statusview-section-text";
  public const string COIN_ICON_NAME = "statusview-coin-icon";
  Label coinLabel;
  public int CoinCount {
    get => this.coinCount;
    set {
      if (value != this.coinCount) {
        this.coinLabel.text = value.ToString();
      }
      this.coinCount = value;
    }
  }
  int coinCount;

  public StatusView()
  {
    this.name = StatusView.CONTAINER_NAME;
    this.CreateUI();
  }

  void CreateUI()
  {
    var coinContainer = new VisualElement();
    coinContainer.AddToClassList(StatusView.SECTION_CLASS);
    var coinIcon = new VisualElement();
    coinIcon.name = StatusView.COIN_ICON_NAME;
    coinIcon.AddToClassList(StatusView.SECTION_ICON_CLASS);
    this.coinLabel = new Label(); 
    this.coinLabel.text = this.CoinCount.ToString();
    this.coinLabel.AddToClassList(StatusView.SECTION_TEXT_CLASS);
    coinContainer.Add(coinIcon);
    coinContainer.Add(this.coinLabel);
    this.Add(coinContainer);
  }
}
