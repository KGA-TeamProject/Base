using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPotion : MonoBehaviour
{
    public Potion potionData; // ScriptableObject ����

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Inventory inventory = collision.gameObject.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(potionData); // �����͸� �߰�
                Destroy(gameObject);
            }
        }
    }
}
