using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] private int Hp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int noticeRadius;
    
    [SerializeField] private int bumpDamage;

    [SerializeField] LayerMask targetLayer;

    

    // ���� �̵��� ������ Attack������Ʈ���� ����
    public void Move()
    {
        if (Physics.OverlapSphere(transform.position, noticeRadius, targetLayer).Length > 0)
        {   
            // �÷��̾�� �ϳ��� �����ϹǷ�
            Transform target = Physics.OverlapSphere(transform.position, noticeRadius, targetLayer)[0].transform;

            // ���� �÷��̾ �ٶ󺸰�
            transform.LookAt(target);

            // ��ֹ��� ������ �̵�
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
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

    private void OnCollisionEnter(Collision collision)
    {

        // Playerü�� ���� �Լ�
        // ���� Damage�� int�� ��� ��
        PlayerHealthBar PlayerHealthBar = collision.gameObject.GetComponent<PlayerHealthBar>();
        if (PlayerHealthBar != null)
        {
            PlayerHealthBar.TakeDamage();
        }

        
    }

}
