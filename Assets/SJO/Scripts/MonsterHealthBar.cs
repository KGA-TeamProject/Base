using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;

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
            // ���� ���
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
