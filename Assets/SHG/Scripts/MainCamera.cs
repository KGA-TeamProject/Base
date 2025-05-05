using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
  const float CAMERA_HEIGHT = 15f;
  const float CAMERA_DEPTH = 5f;
  public Transform Player { 
    get => this.player;
    set {
      this.player = value;
    }
  }
  Transform player;
  // Start is called before the first frame update
  void Start()
  {

  }

  void LateUpdate()
  {
    if (GameManager.Shared.IsPlaying && 
        GameManager.Shared.State == GameManager.GameState.InCombat &&
        this.Player != null) {
      this.FollowPlayer();
    }
  }

  void FollowPlayer()
  {
    this.transform.position = new (
        this.Player.position.x,
        this.Player.position.y + MainCamera.CAMERA_HEIGHT,
        this.Player.position.z - MainCamera.CAMERA_DEPTH
        );
    this.transform.LookAt(this.Player.position);
  }
}
