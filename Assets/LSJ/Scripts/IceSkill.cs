using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSkill : Item
{
    Player player; // �÷��̾� ��ü
    Inventory inventory; // �κ��丮 ��ü

    public IceSkill()
    {
        itemName = "���� ȭ��"; // ������ �̸� ����
        itemDescription = "�������� �� ȭ���� �߻�"; // ������ ���� ����
    }

    public override void Use()
    {
        Debug.Log("��ų ����"); 
        //inventory.AddItem(new IceArrow()); // �κ��丮�� ���� ȭ�� �߰�

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>(); // �÷��̾� ������Ʈ ��������
            Use(); // 
            Destroy(gameObject); // ���� ������Ʈ �ı�
        }
    }
}
