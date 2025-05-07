using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    [SerializeField] Slider nextHPBar;

    public bool nextHPHit = false;

    // 체력을 알 수 있는 변수 생성
    public float maxHP = 100;
    public float currentHP = 100;

    void Update()
    {
        // 뚝뚝 끊기는 모션으로 인해 선형 보간 사용
        hpBar.value = Mathf.Lerp(hpBar.value, currentHP / maxHP, Time.deltaTime * 3f);

        // 뒷쪽 슬라이드가 움직이도록 선형 보간 사용
        // 만약 뒷쪽 슬라이드가 움직인다면
        if (nextHPHit)
        {
            nextHPBar.value = Mathf.Lerp(nextHPBar.value, hpBar.value, Time.deltaTime * 10f);

            // 동작 후 초기화 진행
            // 앞 hpBar와 뒤 hpBar가 가까워지면
            if (hpBar.value >= nextHPBar.value - 0.01f)
            {
                // 초기화
                nextHPHit = false;
                // 앞 뒤 hpBar 위치를 동일하게
                nextHPBar.value = hpBar.value;
            }
        }
    }

    // 이 자체로 동작하지 않을시
    // 몬스터 HP 깎는 쪽에 추가하면 됨
    // OnCollisionEnter 쪽에 TakeDamage();
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
    // 현재 체력과 최대 체력을 연결
    //public void SetUpHP(float amount)
    //{
    //    maxHP = amount;
    //    currentHP = maxHP;
    //}
    //
    //// 체력을 체크 할 함수
    //public void CheckHP()
    //{
    //    // 만약 체력바가 null이 아니면
    //    if (hpBar != null)
    //    {
    //        // 체력바 값을 현재 체력 나누기 최대 체력으로
    //        hpBar.value = (float)currentHP / (float)maxHP;
    //    }
    //}
    //
    //// 데미지를 받을 수 있는 함수
    //public void TakeDamage(float damageTaken)
    //{
    //    // 만약 최대 체력과 현재 체력이 0이거나 0보다 작으면 return
    //    if (maxHP == 0 || currentHP <= 0)
    //    {
    //        return;
    //    }
    //    // 현재 체력에서 받은 데미지 차감
    //    currentHP -= damageTaken;
    //
    //    if (currentHP <= 0)
    //    {
    //        // 플레이어 사망
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
