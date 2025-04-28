using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
{
    Player player; // 플레이어 객체

    public Potion()
    {
        itemName = "힐링 포션"; // 아이템 이름 설정
        itemDescription = "소량의 체력을 회복하는 아이템"; // 아이템 설명 설정
    }

    public override void Use()
    {
        player.Heal(30); // 플레이어 체력 회복
    }
}
