using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
{
    Player player; // �÷��̾� ��ü
    private float rotSpeed = 100f;        // ���ư��� �ӵ�

    public Potion()
    {
        itemName = "���� ����"; // ������ �̸� ����
        itemDescription = "�ҷ��� ü���� ȸ���ϴ� ������"; // ������ ���� ����
    }

    public override void Use()
    {
        Debug.Log("���� ��� ü�� 30 ȸ��"); // ���� ��� �α� ���
        player.Heal(30); // �÷��̾� ü�� ȸ��
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>(); // �÷��̾� ������Ʈ ��������
            Inventory inventory = collision.gameObject.GetComponentInChildren<Inventory>();
            inventory.AddItem(this); 
            Use(); // ���� ���
            //Destroy(gameObject); // ���� ������Ʈ �ı�
        }
    }

    void Update()
    {
        Rotate(); // ���� ȸ��
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);    // Vector3.up �� �������� ����. -> Up ���� ����!���� ���� �� 
    }
}
