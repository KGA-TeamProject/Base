using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerMove2 : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotate;

    private void Update()
    {
        Move();
    }

    // �������� ������ Move �Լ�
    // Lerp�� ���� ���� ����
    private void Move()
    {
        Vector3 direction = Direction();
        if (direction == Vector3.zero)
        {
            return;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotate * Time.deltaTime);
        transform.position += moveSpeed * Time.deltaTime * direction;
    }

    // ���� ��Ʈ���� ���� Vector3
    private Vector3 Direction()
    {
        // Vector3 ��ġ �ʱ�ȭ
        Vector3 userInputKey = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            userInputKey.z += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            userInputKey.z -= 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            userInputKey.x -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            userInputKey.x += 1;
        }

        // ����ȭ
        return userInputKey.normalized;
    }
}
