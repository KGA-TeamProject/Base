using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
{
    Player player; // �÷��̾� ��ü

    public Potion()
    {
        itemName = "���� ����"; // ������ �̸� ����
        itemDescription = "�ҷ��� ü���� ȸ���ϴ� ������"; // ������ ���� ����
    }

    public override void Use()
    {
        player.Heal(30); // �÷��̾� ü�� ȸ��
    }
}
