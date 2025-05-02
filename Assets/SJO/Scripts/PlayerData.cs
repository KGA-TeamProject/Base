using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    [SerializeField] Slider PlayerExpBar;

    // �÷��̾� �ʱ� ���� ����
    public int PlayerLevel = 1;
    // �÷��̾� ���� ����ġ ����
    public float PlayerCurrentExp = 0f;
    // �������� �ʿ��� ������ ����
    public float PlayerLvUpExp = 100f;

    // �÷��̾� ������ �Լ�
    public void PlayerExp(float exp)
    {
        PlayerCurrentExp += exp;

        // ���� �÷��̾��� ���� ����ġ�� �������� �ʿ��� ����ġ���� ũ�ٸ�
        if (PlayerCurrentExp >= PlayerLvUpExp)
        {
            // ���� 1 ���ϱ�
            PlayerLevel++;
            // ���� ����ġ�� �ʱ�ȭ
            PlayerCurrentExp -= PlayerLvUpExp;
        }
    }
}
