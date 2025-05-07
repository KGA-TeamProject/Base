using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public Inventory inventory; // �÷��̾��� �κ��丮

    private int curHP; // �÷��̾��� ü��
    public int CurHP // �÷��̾��� ���� ü��
    {
        get { return curHP; } // ���� ü�� ��ȯ
        set { curHP = value; } // ���� ü�� ����
    }
    private int maxHP; // �÷��̾��� �ִ� ü��
    public int MaxHP // �÷��̾��� �ִ� ü��
    {
        get { return maxHP; } // �ִ� ü�� ��ȯ
        set { maxHP = value; } // �ִ� ü�� ����
    }

    public Player()
    {    
        maxHP = 100; // �ִ� ü�� �ʱ�ȭ
        curHP = maxHP; // ���� ü�� �ʱ�ȭ
    }

    public void Heal(int amount)
    {
        curHP += amount; // ü�� ȸ��
        if (curHP > maxHP) // �ִ� ü�� �ʰ� ����
        {
            curHP = maxHP; // �ִ� ü������ ����
        }
    }


}
