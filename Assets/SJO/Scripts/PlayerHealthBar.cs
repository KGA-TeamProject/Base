using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    
    #region ���� �� ü�¹� �ڵ�
    //private int hp;
    //
    //public int playerHP
    //{
    //    // get���� PlayerHealthBar �̿ܿ� �������� ���ϵ��� ����
    //    get => hp;
    //    // �ּ� / �ִ밪 �������� int�� 0�� ������ ���� �ʵ��� ����
    //    private set => hp = Mathf.Clamp(value, 0, hp);
    //}
    //
    //// Awake �ܰ迡�� �ʱ� ����
    //private void Awake()
    //{
    //    // �ʱ� ü�� ����
    //    hp = 100;
    //    SetMaxHealth(hp);
    //}
    //
    //// ü�� �ִ밪 ������ ���� �Լ� ����
    //public void SetMaxHealth(int playerHealth)
    //{
    //    hpBar.maxValue = playerHealth;
    //    hpBar.value = playerHealth;
    //}
    //
    //// ������ ���ݷ� ��ŭ HP�� �Ҵ�
    //public void TakenDamage(int damage)
    //{
    //    int damageTaken = playerHP - damage;
    //    playerHP = damageTaken;
    //    hpBar.value = playerHP;
    //}
    #endregion


}
