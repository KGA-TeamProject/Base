using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;

    // 빌드업 사용
    private void LateUpdate()
    {
        // 바라보는 방향을 카메라 방향으로 설정
        Vector3 pos = Camera.main.WorldToScreenPoint(player.position);
        
        pos += offset;
        pos.z -= 2f;

        transform.position = Camera.main.ScreenToWorldPoint(pos);
        transform.forward = Camera.main.transform.forward;
    }

    #region 다른 예시
    //Transform camera;
    //
    //private void Start()
    //{
    //    // 시작시 카메라의 위치를 메인 카메라의 위치로
    //    camera = Camera.main.transform;
    //}
    //
    //// LateUpdate에서 체력바가 카메라를 바라보도록
    //private void LateUpdate()
    //{
    //    transform.LookAt(transform.position + camera.rotation * Vector3.forward, camera.rotation * Vector3.up);
    //
    //    // 이렇게만 적어도 상관없음
    //    transform.forward = Camera.main.transform.forward;
    //}
    #endregion
}
