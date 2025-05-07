using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] private int Hp;
    [SerializeField] private float moveSpeed;
   
    
    [SerializeField] private int bumpDamage;

    [SerializeField] LayerMask targetLayer;

    

    // 몬스터 이동을 각각의 Attack컴포넌트에서 참조
    public void Move(Transform target)
    {   
        // 먼저 플레이어를 바라보고
        transform.LookAt(target);

        // 장애물이 없으면 이동
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) )
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                animator.Play("Move");
                transform.position = Vector3.MoveTowards(
                        transform.position,
                        target.position,
                        moveSpeed * Time.deltaTime);
            }
            else 
            {
                animator.Play("Idle");
            }
        }
    }



    public void TakeDamage(int damage)
    {
        Hp -= damage;
        animator.Play("GetHit");
        if (Hp <= 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        animator.Play("Die");
        Destroy(gameObject);
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        // Player체력 감소 함수
        // 현재 Damage는 int로 사용 중
    }*/

}
