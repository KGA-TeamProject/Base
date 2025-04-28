using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatData : MonoBehaviour
{
    // �÷��̾� �ִ� ü�� ����
    [SerializeField] int playerMaxHP = 100;

    // �÷��̾� ���� ü�� ����
    [SerializeField] int playerCurrentHP;


    // ���� ��, �÷��̾� ���� ü���� �ִ� ü�� ����
    private void Awake()
    {
        playerCurrentHP = playerMaxHP;
    }


}
