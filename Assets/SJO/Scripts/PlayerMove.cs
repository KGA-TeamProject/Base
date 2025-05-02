using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // �÷��̾� �̵��ӵ��� ���� ����
    [SerializeField] float _moveSpeed;
    [SerializeField] Rigidbody rigid;

    private Vector3 userInputVec;

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        userInputVec = new Vector3(x, 0, z).normalized;
    }

    private void Move()
    {
        rigid.velocity = userInputVec * _moveSpeed;
    }
}
