using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSkill : Skill
{
    Player player; // �÷��̾� ��ü
    private float rotSpeed = 100f;        // ���ư��� �ӵ�

    public IceSkill()
    {
        SkillName = "���� ȭ��"; // ��ų �̸� ����
        SkillDescription = "�������� �� ȭ���� �߻�"; // ��ų ���� ����
    }

    public void Use()
    {
        Debug.Log("ice skill ����"); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>(); // �÷��̾� ������Ʈ ��������
            SkillContainer skillContainer = collision.gameObject.GetComponentInChildren<SkillContainer>();
            skillContainer.AddSkill(this);
            Use(); // ���
            //Destroy(gameObject); //������Ʈ �ı�
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
