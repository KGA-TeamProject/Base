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

    

    // ���� �̵��� ������ Attack������Ʈ���� ����
    public void Move(Transform target)
    {   
        // ���� �÷��̾ �ٶ󺸰�
        transform.LookAt(target);

        // ��ֹ��� ������ �̵�
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
        // Playerü�� ���� �Լ�
        // ���� Damage�� int�� ��� ��
    }*/

}
