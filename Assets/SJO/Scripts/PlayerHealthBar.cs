using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    
    #region 변경 전 체력바 코드
    //private int hp;
    //
    //public int playerHP
    //{
    //    // get으로 PlayerHealthBar 이외에 접근하지 못하도록 설정
    //    get => hp;
    //    // 최소 / 최대값 설정으로 int가 0의 범위를 넘지 않도록 설정
    //    private set => hp = Mathf.Clamp(value, 0, hp);
    //}
    //
    //// Awake 단계에서 초기 세팅
    //private void Awake()
    //{
    //    // 초기 체력 설정
    //    hp = 100;
    //    SetMaxHealth(hp);
    //}
    //
    //// 체력 최대값 설정을 위한 함수 세팅
    //public void SetMaxHealth(int playerHealth)
    //{
    //    hpBar.maxValue = playerHealth;

    //}
    //
    //// 몬스터의 공격력 만큼 HP에 할당
    //public void TakenDamage(int damage)
    //{
    //    int damageTaken = playerHP - damage;
    //    playerHP = damageTaken;
    //    hpBar.value = playerHP;
    //}
    #endregion

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
            // 플레이어 사망
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
