using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSkill : Skill
{
    Player player; // 플레이어 객체
    private float rotSpeed = 100f;        // 돌아가는 속도

    public IceSkill()
    {
        SkillName = "얼음 화살"; // 스킬 이름 설정
        SkillDescription = "얼음으로 된 화살을 발사"; // 스킬 설명 설정
    }

    public void Use()
    {
        Debug.Log("ice skill 습득"); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<Player>(); // 플레이어 컴포넌트 가져오기
            SkillContainer skillContainer = collision.gameObject.GetComponentInChildren<SkillContainer>();
            skillContainer.AddSkill(this);
            Use(); // 사용
            //Destroy(gameObject); //오브젝트 파괴
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
