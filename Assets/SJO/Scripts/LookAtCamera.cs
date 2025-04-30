using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // ü�¹ٰ� ���� ī�޶� �ٶ󺸰� �ϱ�

    // ����� ���
    Transform camera;

    private void Start()
    {
        // ���۽� ī�޶��� ��ġ�� ���� ī�޶��� ��ġ��
        camera = Camera.main.transform;
    }

    // LateUpdate���� ü�¹ٰ� ī�޶� �ٶ󺸵���
    private void LateUpdate()
    {
        transform.LookAt(transform.position + camera.rotation * Vector3.forward, camera.rotation * Vector3.up);

        // �̷��Ը� ��� �������
        // transform.forward = Camera.main.transform.forward;
    }

}
