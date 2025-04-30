using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // 체력바가 메인 카메라 바라보게 하기

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
