using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    // �κ��丮 Ŭ����
    // �κ��丮 Ŭ������ �÷��̾��� ������ ����� �����մϴ�.
    // ������ �߰�, ����, ��� ���� ����� �����մϴ�.

    #region Singleton
    private static Inventory instance;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                Inventory prefab = Resources.Load<Inventory>("Inventory"); // �κ��丮 �ε�
                Instantiate(prefab); // �ν��Ͻ� ����
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // �ν��Ͻ� ����
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }
    }
    #endregion

    public List<Item> items;

    // ������ �߰�
    public void AddItem(Item item)
    {
        // ������ �߰� ����
        items.Add(item); // ������ �߰�
        item.gameObject.SetActive(false); // ������ ��Ȱ��ȭ
        item.transform.parent = transform; // ������ �θ� �κ��丮�� ����
    }

    // ������ ���
    public void UseItem(Item item)
    {
        // ������ ��� ����
        item.Use(); // ������ ���
        RemoveItem(item); // ������ ����
    }
    public void UseItem(int index)
    {
        // ������ ��� ����
        Item item = items[index]; // ����Ϸ��� ������
        items.RemoveAt(index); // ������ ��Ͽ��� ����
        item.Use(); // ������ ���
        Destroy(item.gameObject); // ������ ������Ʈ �ı�

    }

    // ������ ��� ����
    public void DropItem(Item item)
    {        
        items.Remove(item); // ������ ��Ͽ��� ����
        item.gameObject.SetActive(true); // ������ Ȱ��ȭ
        item.transform.parent = null; // ������ �θ� null�� ����
    }

    public void DropItem(int index)
    {
        Item item = items[index]; // ����� ������
        items.RemoveAt(index); // ������ ��Ͽ��� ����
        item.gameObject.SetActive(true); // ������ Ȱ��ȭ
        item.transform.parent = null; // ������ �θ� null�� ����
    }


    // ������ ����
    public void RemoveItem(Item item)
    {
        // ������ ���� ����
        items.Remove(item); // ������ ����
    }
    

    // ������ ��� ��ȯ
    //public List<Item> GetItems()
    //{
    //    return items; // ������ ��� ��ȯ
    //}

}
