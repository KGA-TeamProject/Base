using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : Skill
{
    [Header("Conponents")]
    [SerializeField] Rigidbody rigid;

    [Header("Propertis")]
    [SerializeField] GameObject explosionEffectPrefab;

    //public float damage; // 스킬 데미지
    //public float range; // 스킬 사거리
    //public float cooldown; // 스킬 쿨타임

    public FireArrow()
    {
        SkillName = "화염 화살"; // 스킬 이름 설정
        SkillDescription = "화염으로 된 화살을 발사"; // 스킬 설명 설정
    }

    private void Update()
    {
        if (rigid.velocity.magnitude > 2)
        {
            transform.forward = rigid.velocity; // 불릿의 앞방향이 속도 방향으로
        }
    }

    private void OnCollisionEnter(Collision collision) // OnCollisionEnter -> 충돌체가 접촉이 시작될 때
    {
        Destroy(gameObject);    // 삭제하면서
        // Destroys는 삭제 예정 방식이라 시간도 입력가능
        // Destroyimmediate 가 즉시 삭제. 권하지 않음.
        Instantiate(explosionEffectPrefab, transform.position, transform.rotation); // 폭발 이펙트를 보여지게

        //IDamagable damagable = collision.gameObject.GetComponent<IDamagable>(); // 데미지를 받을 수 있는 컴포넌트를 불러옴
        //if (damagable != null)  // 데미지를 받을 수 있는 컴포넌트가 null 이 아닐때
        //{
        //    Debug.Log($"{collision.gameObject.name} 에서 총알이 데미지를 받을 수 있는 컴포넌트를 가져옴");
        //    Attack(damagable);  // 공격. 데미지를 준다.
        //}
        //else
        //{
        //    Debug.Log($"{collision.gameObject.name} 에는 데미지를 받을 수 있는 컴포넌트가 없음");
        //}

        //Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();   // 너 Rigidbody 있어?
        //if (rigidbody != null) // 만약 Rigidbody 가 있으면
        //{
        //    rigidbody.AddForce(transform.forward * 10f, ForceMode.Impulse); // 앞쪽으로 10f만큼 밀께
        //}
    }
}
