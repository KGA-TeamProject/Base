using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : GameObject2
{
    public string itemName; // 아이템 이름
    public string itemDescription; // 아이템 설명

    public override void Interact(Player player)
    {
        // 플레이어가 습득한다.
        player.inventory.AddItem(this); // 플레이어 인벤토리에 아이템 추가
    }

    public abstract void Use();
}
