using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterController : MonoBehaviour
{
    [SerializeField] private int Hp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateInterpolation;
    



    

    public void Move(Transform target)
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


}
