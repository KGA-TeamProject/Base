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

    // ü���� �� �� �ִ� ���� ����
    public float currentHP;
    public float maxHP;

    // ���� ü�°� �ִ� ü���� ����
    public void SetUpHP(float amount)
    {
        maxHP = amount;
        currentHP = maxHP;
    }

    // ü���� üũ �� �Լ�
    public void CheckHP()
    {
        // ���� ü�¹ٰ� null�� �ƴϸ�
        if (hpBar != null)
        {
            // ü�¹� ���� ���� ü�� ������ �ִ� ü������
            hpBar.value = (float)currentHP / (float)maxHP;
        }
    }

    // �������� ���� �� �ִ� �Լ�
    public void TakeDamage(float damageTaken)
    {
        // ���� �ִ� ü�°� ���� ü���� 0�̰ų� 0���� ������ return
        if (maxHP == 0 || currentHP <= 0)
        {
            return;
        }
        // ���� ü�¿��� ���� ������ ����
        currentHP -= damageTaken;

        if (currentHP <= 0)
        {
            // �÷��̾� ���
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
