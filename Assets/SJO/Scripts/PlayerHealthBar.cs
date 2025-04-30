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
    //    hpBar.value = playerHealth;
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


}
