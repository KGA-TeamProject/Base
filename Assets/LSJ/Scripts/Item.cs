using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName; // 아이템 이름
    public string itemDescription; // 아이템 설명

    public abstract void Use();

    //private void OnCollisionEnter(Collision collision)    
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Use();
    //        Destroy(gameObject); // 아이템 사용 후 파괴
    //    }
    //}

}
