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
        //    // ���̽�ƽ ���
        //    // ������ ���̽�ƽ �Է°��� �޾� �̵��� �����ϴ� �Լ��� ����
        //    // �Լ� ȣ�� ���� : �Ű�����(JoystickInput)�� �־��ִ� ���·�
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
            // Ű���� ���
            Move();
        //}
    }


    // �������� ������ Move �Լ�
    // Lerp�� ���� ���� ����
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
