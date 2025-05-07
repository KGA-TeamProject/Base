using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : Item
{
    Player player; // 플레이어 객체
    private float rotSpeed = 100f;        // 돌아가는 속도

    public Potion()
    {
        itemName = "힐링 포션"; // 아이템 이름 설정
        itemDescription = "소량의 체력을 회복하는 아이템"; // 아이템 설명 설정
    }

    public override void Use()
    {
        Debug.Log("포션 사용 체력 30 회복"); // 포션 사용 로그 출력
        player.Heal(30); // 플레이어 체력 회복
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>(); // 플레이어 컴포넌트 가져오기
            Inventory inventory = collision.gameObject.GetComponentInChildren<Inventory>();
            inventory.AddItem(this); 
            Use(); // 포션 사용
            //Destroy(gameObject); // 포션 오브젝트 파괴
        }
    }

    void Update()
    {
        Rotate(); // 포션 회전
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);    // Vector3.up 을 기준으로 돈다. -> Up 위를 기준!으로 도는 것 
    }
}
