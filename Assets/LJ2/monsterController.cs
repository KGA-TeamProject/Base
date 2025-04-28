using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
{
    [SerializeField] private int Hp;
    [SerializeField] private int Wp;


    [SerializeField][Range(0, 1)] private float moveInterpolation;
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

        transform.position = Vector3.Lerp(
            transform.position,
            target.position, 
            moveInterpolation * Time.deltaTime);
    }



}
