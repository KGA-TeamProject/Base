using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;

    // ����� ���
    private void LateUpdate()
    {
        // �ٶ󺸴� ������ ī�޶� �������� ����
        Vector3 pos = Camera.main.WorldToScreenPoint(player.position);
        
        pos += offset;
        pos.z -= 2f;

        transform.position = Camera.main.ScreenToWorldPoint(pos);
        transform.forward = Camera.main.transform.forward;
    }

    #region �ٸ� ����
    //Transform camera;
    //
    //private void Start()
    //{
    //    // ���۽� ī�޶��� ��ġ�� ���� ī�޶��� ��ġ��
    //    camera = Camera.main.transform;
    //}
    //
    //// LateUpdate���� ü�¹ٰ� ī�޶� �ٶ󺸵���
    //private void LateUpdate()
    //{
    //    transform.LookAt(transform.position + camera.rotation * Vector3.forward, camera.rotation * Vector3.up);
    //
    //    // �̷��Ը� ��� �������
    //    transform.forward = Camera.main.transform.forward;
    //}
    #endregion
}
