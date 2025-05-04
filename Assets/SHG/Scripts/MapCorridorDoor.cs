using System;
using UnityEngine;

public class MapCorridorDoor : MonoBehaviour
{
  const float DOOR_ACTIAVATE_DIST = 7f;
  SphereCollider doorCollider;
  public Func<bool> OnActivated;
  bool isActivated;

  public void DestorySelf()
  {
    Destroy(this.gameObject);
  }

  void Awake()
  {
    this.isActivated = false;
    this.gameObject.name = "MapCorridorDoor";
    this.doorCollider = this.gameObject.AddComponent<SphereCollider>();
    this.doorCollider.isTrigger = true;
    this.doorCollider.includeLayers = LayerMask.NameToLayer("Player");
  }
  // Start is called before the first frame update
  void Start()
  {
    this.doorCollider.radius = MapCorridorDoor.DOOR_ACTIAVATE_DIST;
  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter(Collider collider)
  {
    if (!this.isActivated && 
        collider.tag == "Player" &&
        this.OnActivated != null) {
      this.isActivated = true;
      this.OnActivated.Invoke();
    }
  }
}
