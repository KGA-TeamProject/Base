using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName; // ������ �̸�
    public string itemDescription; // ������ ����

    public abstract void Use();

    //private void OnCollisionEnter(Collision collision)    
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        Use();
    //        Destroy(gameObject); // ������ ��� �� �ı�
    //    }
    //}

}
