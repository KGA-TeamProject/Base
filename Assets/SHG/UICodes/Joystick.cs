using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Joystick : VisualElement {

  public Vector2 Input => this.normalizedInput;
  const float HANDLE_RADIOUS = 70f;
  const float RESET_DELAY = 0.3f;
  float handleRadius;
  float resetDelay;
  float maxDist => this.handleRadius * 0.5f;
  Vector2 normalizedInput;
  Button handle;
  bool isHandlePressed;
  Vector2 pressedPosition;
  Vector2 handleOffset;

  public Joystick(float radius = Joystick.HANDLE_RADIOUS, float resetDelay = Joystick.RESET_DELAY) { 
    this.handleRadius = radius;
    this.resetDelay = resetDelay;
    this.normalizedInput = new();
    this.pressedPosition = new();
    this.handleOffset = new();
    this.CreateUI();
  }

  public IEnumerator<Action> CreateResetHandleRoutine(Action onEnded) {
    while (this.handleOffset.magnitude > 0.01f) {
      this.handleOffset = Vector2.Lerp(
          this.handleOffset,
          Vector2.zero,
          10.0f * Time.deltaTime
          );
      this.MoveHandle();
      yield return (null);
    }
    onEnded.Invoke();
  }

  void CreateUI() {
    this.AddToClassList("joystickContainer");
    this.handle = new Button(); 
    this.handle.AddToClassList("joystickHandle");
    this.handle.style.width = this.handleRadius * 2;
    this.handle.style.height = this.handleRadius * 2;
    this.RegisterHandleCallbacks();
    this.Add(this.handle);
  }

  void RegisterHandleCallbacks() {
    this.handle.RegisterCallback<PointerDownEvent>(
        this.OnHandlePress, TrickleDown.TrickleDown);
    this.handle.RegisterCallback<PointerUpEvent>(
        this.OnHandleRelease);
    this.handle.RegisterCallback<PointerMoveEvent>(
      evt => { 
        if(this.isHandlePressed)
          this.OnHandleMove(evt);
      }); 
    this.handle.RegisterCallback<TransitionEndEvent>(evt => {
      this.ClearResetHandle();
        });
    this.handle.RegisterCallback<TransitionCancelEvent>(evt => {
      this.ClearResetHandle();
    });
  }

  void OnHandlePress(PointerDownEvent evt) {
    this.isHandlePressed = true;
    this. AddToClassList("joystickContainer-active");
    this.handle.AddToClassList("joystickHandle-active");
    this.pressedPosition.x = evt.position.x;
    this.pressedPosition.y = evt.position.y;
  }

  void OnHandleRelease(PointerUpEvent evt) {
    this.isHandlePressed = false;
    this.normalizedInput = Vector2.zero;
    this.RemoveFromClassList("joystickContainer-active");
    this.handle.RemoveFromClassList("joystickHandle-active");
    this.ResetHandlePosition();
  }

  void ResetHandlePosition() {
    this.handleOffset = Vector2.zero;
    this.handle.style.transitionProperty = new List<StylePropertyName> { "translate" };
    this.handle.style.transitionDuration = new List<TimeValue>{ this.resetDelay };
    this.MoveHandle();
  }

  void ClearResetHandle() {
    this.handle.schedule.Execute(() => {
      this.handle.style.transitionDuration = null;
      this.handle.style.transitionProperty = null;
    });
  }

  void OnHandleMove(PointerMoveEvent evt) {
    this.handleOffset.x = Math.Clamp(evt.position.x - this.pressedPosition.x, - this.maxDist, this.maxDist);
    this.handleOffset.y = Math.Clamp(evt.position.y - this.pressedPosition.y, - this.maxDist, this.maxDist);
    this.MoveHandle();
    this.normalizedInput = new Vector2(this.handleOffset.x / this.maxDist, - this.handleOffset.y / this.maxDist).normalized;
  }

  void MoveHandle() {
    this.handle.style.translate = new StyleTranslate(
        new Translate(this.handleOffset.x, this.handleOffset.y)
        );
  } 
}
