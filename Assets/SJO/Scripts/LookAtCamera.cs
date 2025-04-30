using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // 체력바가 메인 카메라 바라보게 하기

    // 빌드업 사용
    Transform camera;

    private void Start()
    {
        // 시작시 카메라의 위치를 메인 카메라의 위치로
        camera = Camera.main.transform;
    }

    // LateUpdate에서 체력바가 카메라를 바라보도록
    private void LateUpdate()
    {
        transform.LookAt(transform.position + camera.rotation * Vector3.forward, camera.rotation * Vector3.up);

        // 이렇게만 적어도 상관없음
        // transform.forward = Camera.main.transform.forward;
    }

}
