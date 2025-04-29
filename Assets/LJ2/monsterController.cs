using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
{
    [SerializeField] private int Hp;
    [SerializeField] private float moveSpeed;

    [SerializeField] private int bumpDamage;

    [SerializeField] LayerMask targetLayer;

    public void Move(Transform target)
    {
        transform.LookAt(target);

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
