using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;

    // 체력을 알 수 있는 변수 생성
    public float currentHP;
    public float maxHP;

    // 현재 체력과 최대 체력을 연결
    public void SetUpHP(float amount)
    {
        maxHP = amount;
        currentHP = maxHP;
    }

    // 체력을 체크 할 함수
    public void CheckHP()
    {
        // 만약 체력바가 null이 아니면
        if (hpBar != null)
        {
            // 체력바 값을 현재 체력 나누기 최대 체력으로
            hpBar.value = (float)currentHP / (float)maxHP;
        }
    }

    // 데미지를 받을 수 있는 함수
    public void TakeDamage(float damageTaken)
    {
        // 만약 최대 체력과 현재 체력이 0이거나 0보다 작으면 return
        if (maxHP == 0 || currentHP <= 0)
        {
            return;
        }
        // 현재 체력에서 받은 데미지 차감
        currentHP -= damageTaken;

        if (currentHP <= 0)
        {
            // 몬스터 사망
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
