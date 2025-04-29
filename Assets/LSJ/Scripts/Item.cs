using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : GameObject2
{
    public string itemName; // ������ �̸�
    public string itemDescription; // ������ ����

    public override void Interact(Player player)
    {
        // �÷��̾ �����Ѵ�.
        player.inventory.AddItem(this); // �÷��̾� �κ��丮�� ������ �߰�
    }

    public abstract void Use();
}
