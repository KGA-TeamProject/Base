using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public Inventory inventory; // 플레이어의 인벤토리

    private int curHP; // 플레이어의 체력
    public int CurHP // 플레이어의 현재 체력
    {
        get { return curHP; } // 현재 체력 반환
        set { curHP = value; } // 현재 체력 설정
    }
    private int maxHP; // 플레이어의 최대 체력
    public int MaxHP // 플레이어의 최대 체력
    {
        get { return maxHP; } // 최대 체력 반환
        set { maxHP = value; } // 최대 체력 설정
    }

    public Player()
    {    
        maxHP = 100; // 최대 체력 초기화
        curHP = maxHP; // 현재 체력 초기화
    }

    public void Heal(int amount)
    {
        curHP += amount; // 체력 회복
        if (curHP > maxHP) // 최대 체력 초과 방지
        {
            curHP = maxHP; // 최대 체력으로 설정
        }
    }


}
