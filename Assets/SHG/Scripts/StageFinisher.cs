using System;
using UnityEngine;

public class StageFinisher : MonoBehaviour
{
  const float TRIGGER_RADIUS = 1f;
  SphereCollider trigger;
  public Action OnActivate;

  public void DestroySelf()
  {
    Destroy(this.gameObject);
  }

  public void Hide()
  {
    this.trigger.enabled = false;
    this.gameObject.SetActive(false);
  }

  public void Show()
  {
    this.trigger.enabled = true;
    this.gameObject.SetActive(true);
  }

  void Awake()
  {
    this.trigger = this.gameObject.AddComponent<SphereCollider>();
    this.trigger.isTrigger = true;
    this.trigger.radius = StageFinisher.TRIGGER_RADIUS;
    this.trigger.includeLayers = LayerMask.NameToLayer("Player");
  }
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter(Collider collider) 
  {
    if (collider.tag == "Player" && this.OnActivate != null) {
      this.OnActivate.Invoke();
    }
  }
}
