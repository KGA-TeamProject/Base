using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
{
    [SerializeField] private int Hp;
    [SerializeField] private float moveSpeed;

    [SerializeField] private int bumpDamage;

    public void Move(Transform target)
    {
        transform.LookAt(target);

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime);
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
