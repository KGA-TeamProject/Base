using UnityEngine;
using UnityEngine.UI;

public class PlayerMove2 : MonoBehaviour
{
    public bool isMove;

    [SerializeField] float moveSpeed;
    [SerializeField] float rotate;

   

    private void Update()
    {
        //Vector2 joystickInput = UIManager.Shared.JoystickInput;
        //if (joystickInput != Vector2.zero) 
        //{
        //    // 조이스틱 사용
        //    // 별도로 조이스틱 입력값을 받아 이동을 수행하는 함수를 선언
        //    // 함수 호출 조건 : 매개변수(JoystickInput)을 넣어주는 형태로
        //    Vector3 direction = new Vector3 (joystickInput.x, 0, joystickInput.y);
        //    if (direction == Vector3.zero)
        //    {
        //        isMove = false;
        //        return;
        //    }
        //    else
        //    {
        //        isMove = true;
        //        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotate * Time.deltaTime);
        //        transform.position += moveSpeed * Time.deltaTime * direction;
        //    }
        //
        //}
        //else 
        //{
            // 키보드 사용
            Move();
        //}
    }


    // 움직임을 구현할 Move 함수
    // Lerp를 통해 구현 예정
    private void Move()
    {
        Vector3 direction = Direction();
        if (direction == Vector3.zero)
        {
            isMove = false;
            return;
        }
        else
        {
            isMove = true;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotate * Time.deltaTime);
            transform.position += moveSpeed * Time.deltaTime * direction;
        }

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
