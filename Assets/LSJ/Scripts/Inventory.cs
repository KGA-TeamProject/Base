using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    private List<Item> items;

    public Inventory() // �κ��丮 ������
    {
        new List<Item>() { }; // ������ ���
    } 

    // ������ �߰�
    public void AddItem(Item item)
    {
        // ������ �߰� ����
        items.Add(item); // ������ �߰�
    }
    // ������ ����
    public void RemoveItem(Item item)
    {
        // ������ ���� ����
        items.Remove(item); // ������ ����
    }
    // ������ ���
    public void UseItem(Item item)
    {
        // ������ ��� ����
        item.Use(); // ������ ���
    }

    // ������ ��� ��ȯ
    //public List<Item> GetItems()
    //{
    //    return items; // ������ ��� ��ȯ
    //}

}
