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

    // 움직임을 구현할 Move 함수
    // Lerp를 통해 구현 예정
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

    // 실제 컨트롤을 위한 Vector3
    private Vector3 Direction()
    {
        // Vector3 위치 초기화
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

        // 정규화
        return userInputKey.normalized;
    }
}
