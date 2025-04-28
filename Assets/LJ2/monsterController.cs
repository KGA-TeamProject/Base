using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
{
    [SerializeField] private int Hp;
    [SerializeField] private int nearAp;
    [SerializeField] private int farAp;
    [SerializeField] private float attackRadius;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateInterpolation;
    //[SerializeField] private float attackSpeed;
    [SerializeField] Transform target;



    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(target.position),
            rotateInterpolation * Time.deltaTime
            );

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

    }

}
