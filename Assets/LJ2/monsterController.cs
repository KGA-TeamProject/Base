using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
{
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
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target.position,
                    moveSpeed * Time.deltaTime);
            }
        }
    }



    private void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp < 0)
        {
            Die();
        }
    }

    private void Die() 
    {
        Destroy(gameObject);
    }
}
