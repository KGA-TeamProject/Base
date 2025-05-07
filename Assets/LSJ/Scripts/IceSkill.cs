using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSkill : Item
{
    Player player; // 플레이어 객체
    Inventory inventory; // 인벤토리 객체

    public IceSkill()
    {
        itemName = "얼음 화살"; // 아이템 이름 설정
        itemDescription = "얼음으로 된 화살을 발사"; // 아이템 설명 설정
    }

    public override void Use()
    {
        Debug.Log("스킬 습득"); 
        //inventory.AddItem(new IceArrow()); // 인벤토리에 얼음 화살 추가

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>(); // 플레이어 컴포넌트 가져오기
            Use(); // 
            Destroy(gameObject); // 포션 오브젝트 파괴
        }
    }
}
