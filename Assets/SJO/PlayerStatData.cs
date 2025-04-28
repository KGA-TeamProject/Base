using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatData : MonoBehaviour
{
    // 플레이어 최대 체력 설정
    [SerializeField] int playerMaxHP = 100;

    // 플레이어 현재 체력 설정
    [SerializeField] int playerCurrentHP;


    // 시작 시, 플레이어 현재 체력은 최대 체력 설정
    private void Awake()
    {
        playerCurrentHP = playerMaxHP;
    }


}
