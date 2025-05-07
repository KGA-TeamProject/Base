using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    [SerializeField] Slider nextHPBar;

    public bool nextHPHit = false;

    // ü���� �� �� �ִ� ���� ����
    public float maxHP = 100;
    public float currentHP = 100;

    void Update()
    {
        // �Ҷ� ����� ������� ���� ���� ���� ���
        hpBar.value = Mathf.Lerp(hpBar.value, currentHP / maxHP, Time.deltaTime * 3f);

        // ���� �����̵尡 �����̵��� ���� ���� ���
        // ���� ���� �����̵尡 �����δٸ�
        if (nextHPHit)
        {
            nextHPBar.value = Mathf.Lerp(nextHPBar.value, hpBar.value, Time.deltaTime * 10f);

            // ���� �� �ʱ�ȭ ����
            // �� hpBar�� �� hpBar�� ���������
            if (hpBar.value >= nextHPBar.value - 0.01f)
            {
                // �ʱ�ȭ
                nextHPHit = false;
                // �� �� hpBar ��ġ�� �����ϰ�
                nextHPBar.value = hpBar.value;
            }
        }
    }

    // �� ��ü�� �������� ������
    // ���� HP ��� �ʿ� �߰��ϸ� ��
    // OnCollisionEnter �ʿ� TakeDamage();
    public void TakeDamage()
    {
        currentHP -= 10;
        Invoke("NextHP", 0.5f);
    }

    public void NextHP()
    {
        nextHPHit = true;
    }

    #region Before
    // ���� ü�°� �ִ� ü���� ����
    //public void SetUpHP(float amount)
    //{
    //    maxHP = amount;
    //    currentHP = maxHP;
    //}
    //
    //// ü���� üũ �� �Լ�
    //public void CheckHP()
    //{
    //    // ���� ü�¹ٰ� null�� �ƴϸ�
    //    if (hpBar != null)
    //    {
    //        // ü�¹� ���� ���� ü�� ������ �ִ� ü������
    //        hpBar.value = (float)currentHP / (float)maxHP;
    //    }
    //}
    //
    //// �������� ���� �� �ִ� �Լ�
    //public void TakeDamage(float damageTaken)
    //{
    //    // ���� �ִ� ü�°� ���� ü���� 0�̰ų� 0���� ������ return
    //    if (maxHP == 0 || currentHP <= 0)
    //    {
    //        return;
    //    }
    //    // ���� ü�¿��� ���� ������ ����
    //    currentHP -= damageTaken;
    //
    //    if (currentHP <= 0)
    //    {
    //        // �÷��̾� ���
    //        Die();
    //    }
    //}
    //
    //private void Die()
    //{
    //    Destroy(gameObject);
    //}
    #endregion
}
