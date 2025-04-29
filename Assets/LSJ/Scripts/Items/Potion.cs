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
        Debug.Log("���� ���"); // ���� ��� �α� ���
        player.Heal(30); // �÷��̾� ü�� ȸ��
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>(); // �÷��̾� ������Ʈ ��������
            Use(); // ���� ���
            Destroy(gameObject); // ���� ������Ʈ �ı�
        }
    }
}
