using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;

    private int hp;

    private int playerHP
    {
        // get���� PlayerHealthBar �̿ܿ� �������� ���ϵ��� ����
        get => hp;
        // �ּ� / �ִ밪 �������� int�� 0�� ������ ���� �ʵ��� ����
        set => hp = Mathf.Clamp(value, 0, hp);
    }


}
