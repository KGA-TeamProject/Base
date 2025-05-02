using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    [SerializeField] Slider PlayerExpBar;

    // 플레이어 초기 레벨 설정
    public int PlayerLevel = 1;
    // 플레이어 현재 경험치 설정
    public float PlayerCurrentExp = 0f;
    // 레벨업에 필요한 경험이 설정
    public float PlayerLvUpExp = 100f;

    // 플레이어 레벨업 함수
    public void PlayerExp(float exp)
    {
        PlayerCurrentExp += exp;

        // 만약 플레이어의 현재 경험치가 레벨업에 필요한 경험치보다 크다면
        if (PlayerCurrentExp >= PlayerLvUpExp)
        {
            // 레벌 1 더하기
            PlayerLevel++;
            // 현재 경험치를 초기화
            PlayerCurrentExp -= PlayerLvUpExp;
        }
    }
}
