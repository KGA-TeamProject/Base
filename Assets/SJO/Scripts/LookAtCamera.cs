using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // ü�¹ٰ� ���� ī�޶� �ٶ󺸰� �ϱ�

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
