using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : Skill
{
    [Header("Conponents")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] private float bulletSpeed; // 총알이 나아가는 힘

    [Header("Propertis")]
    [SerializeField] GameObject explosionEffectPrefab;

    public Stack<GameObject> returnPool; // 오브젝트 풀을 관리할 스택 -> 아직 정해지지 않음

    //public float damage; // 스킬 데미지
    //public float range; // 스킬 사거리
    //public float cooldown; // 스킬 쿨타임

    public IceArrow()
    {
        SkillName = "얼음 화살"; // 스킬 이름 설정
        SkillDescription = "얼음으로 된 화살을 발사"; // 스킬 설명 설정
    }

    void OnEnable()
    {
        // 활성화 될때마다 앞으로 나가게끔. OnEnable
        rigid.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse); // 총알이 나아가는 힘을 준다. -> transform.forward는 총알이 나아가는 방향을 의미한다.
        StartCoroutine(ReturnPool(3f)); // 코루틴을 시작한다. -> 총알이 발사된 후, 일정 시간이 지나면 삭제한다.
    }

    private void Update()
    {
        if (rigid.velocity.magnitude > 2)
        {
            transform.forward = rigid.velocity; // 화살의 앞방향이 속도 방향으로
        }

        // 게임이 끝났을때 -> 게임매니저에서 상태를 받아서.
        //if ()
        //{
        //    StartCoroutine(ReturnPool());
        //}
    }

    // 언제언제 리턴해줘야할까?
    // 시간이 지났을때.
    // 부딪혔을때.
    private IEnumerator ReturnPool(float delay = 0f)
    {
        yield return new WaitForSeconds(delay); // 3초 후에 실행된다. -> 총알이 발사된 후, 일정 시간(3초)이 지나면 삭제한다.
        rigid.velocity = Vector3.zero; // 화살의 속도를 0으로 만든다. -> 화살의 속도 초기화.
        gameObject.SetActive(false); // 화살 비활성화한다. -> 화살이 발사된 후, 일정 시간이 지나면 삭제한다.
        returnPool.Push(gameObject); // 오브젝트 풀에 돌려보내기.
    }

    private void OnTriggerEnter(Collider other)    // 몬스터와 충돌하면, 삭제
    {
        if (other.gameObject.layer == 8) // 몬스터와 충돌했을 때
        {
            //Destroy(gameObject); // 총알을 삭제한다.

            StartCoroutine(ReturnPool()); // 오브젝트 풀에 추가한다. -> 총알이 발사된 후, 일정 시간이 지나면 삭제한다.
        }
    }


    //private void OnCollisionEnter(Collision collision) // OnCollisionEnter -> 충돌체가 접촉이 시작될 때
    //{
    //    Destroy(gameObject);    // 삭제하면서
    //    // Destroys는 삭제 예정 방식이라 시간도 입력가능
    //    // Destroyimmediate 가 즉시 삭제. 권하지 않음.
    //    Instantiate(explosionEffectPrefab, transform.position, transform.rotation); // 폭발 이펙트를 보여지게

    //    //IDamagable damagable = collision.gameObject.GetComponent<IDamagable>(); // 데미지를 받을 수 있는 컴포넌트를 불러옴
    //    //if (damagable != null)  // 데미지를 받을 수 있는 컴포넌트가 null 이 아닐때
    //    //{
    //    //    Debug.Log($"{collision.gameObject.name} 에서 총알이 데미지를 받을 수 있는 컴포넌트를 가져옴");
    //    //    Attack(damagable);  // 공격. 데미지를 준다.
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log($"{collision.gameObject.name} 에는 데미지를 받을 수 있는 컴포넌트가 없음");
    //    //}

    //    //Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();   // 너 Rigidbody 있어?
    //    //if (rigidbody != null) // 만약 Rigidbody 가 있으면
    //    //{
    //    //    rigidbody.AddForce(transform.forward * 10f, ForceMode.Impulse); // 앞쪽으로 10f만큼 밀께
    //    //}
    //}


}

