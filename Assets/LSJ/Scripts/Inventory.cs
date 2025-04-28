using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    private List<Item> items;

    public Inventory() // 인벤토리 생성자
    {
        new List<Item>() { }; // 아이템 목록
    } 

    // 아이템 추가
    public void AddItem(Item item)
    {
        // 아이템 추가 로직
        items.Add(item); // 아이템 추가
    }
    // 아이템 제거
    public void RemoveItem(Item item)
    {
        // 아이템 제거 로직
        items.Remove(item); // 아이템 제거
    }
    // 아이템 사용
    public void UseItem(Item item)
    {
        // 아이템 사용 로직
        item.Use(); // 아이템 사용
    }

    // 아이템 목록 반환
    //public List<Item> GetItems()
    //{
    //    return items; // 아이템 목록 반환
    //}

}
