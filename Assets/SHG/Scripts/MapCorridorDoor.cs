using System;
using UnityEngine;

public class MapCorridorDoor : MonoBehaviour
{
  const float DOOR_ACTIAVATE_DIST = 7f;
  public Action<Action> OnActivated;
  public MapTypes.TileDirection Dir;
  SphereCollider doorTrigger;
  Collider doorCollider;
  bool isActivated;
  Transform left;
  Transform right;

  public void SetAsUnActivated()
  {
    this.isActivated = false;
    this.gameObject.SetActive(true);
    this.Close();
  }

  public void DestorySelf()
  {
    Destroy(this.gameObject);
  }

  public void Hide()
  {
    this.gameObject.SetActive(false);
  }

  void Awake()
  {
    this.isActivated = false;
    this.gameObject.name = "MapCorridorDoor";
    this.doorCollider = this.gameObject.GetComponent<Collider>();
    this.doorTrigger = this.gameObject.AddComponent<SphereCollider>();
    this.doorTrigger.isTrigger = true;
    this.doorTrigger.includeLayers = LayerMask.NameToLayer("Player");
    var container = this.transform.Find("Container");
    this.left = this.transform.Find("Left") ?? container?.transform.Find("Left");
    this.right = this.transform.Find("Right") ?? container?.transform.Find("Right");
  }
  // Start is called before the first frame update
  void Start()
  {
    this.doorTrigger.radius = MapCorridorDoor.DOOR_ACTIAVATE_DIST;
    this.SetGeometry();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void SetGeometry()
  {
    Vector3 offset = new ();
    Vector3 rotation = new ();
    switch (this.Dir) {
      case MapTypes.TileDirection.Top:
        offset.z -= 0.3f;
        break;
      case MapTypes.TileDirection.Bottom:
        offset.z += 0.3f;
        break;
      case MapTypes.TileDirection.Left:
        offset.x += 0.3f;
        rotation.y = 90f;
        break;
      case MapTypes.TileDirection.Right:
        offset.x -= 0.3f;
        rotation.y = 90f;
        break;
    }
    this.transform.position += offset;
    this.transform.Rotate(rotation);
  }
  
  void OnCollisionEnter(Collision collision) {
    if (this.isActivated && collision.collider.tag == "Player") {
      this.Open();
    }
  }

  void OnTriggerEnter(Collider collider)
  {
    if (!this.isActivated && 
        collider.tag == "Player" &&
        this.OnActivated != null) {
      this.OnActivated.Invoke(() => this.isActivated = true);
    }
  }

  void Open()
  {
    if (this.doorCollider != null) {
      this.doorCollider.enabled = false;
    }
    if (this.left != null && this.right != null) {
      this.left.transform.Rotate(new Vector3(0, 90, 0), Space.World);
      this.right.transform.Rotate(new Vector3(0, - 90, 0), Space.World);
    }
  }

  void Close()
  {
    if (this.doorCollider != null) {
      this.doorCollider.enabled = true;
    }
    if (this.left != null && this.right != null) {
      this.left.transform.rotation = Quaternion.identity;
      this.right.transform.rotation = Quaternion.identity;
    }
  }
}
