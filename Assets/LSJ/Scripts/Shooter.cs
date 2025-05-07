using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab; // 총알 프리팹
    [SerializeField] Transform muzzlePoint; // 총알 발사 위치

    [Range(15, 40)]
    [SerializeField] float bulletSpeed; // 발사 속도


    public void Fire()
    {
        // 총알을 발사하는 메서드
        GameObject instance = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation); // 총알 인스턴스 생성
                                                                                                     // 머즐 포인트 위치와 회전으로 총알 생성
        Destroy(instance, 3); // 3초 후 총알 삭제

        Rigidbody bulletRigidbody = instance.GetComponent<Rigidbody>(); // 총알의 리지드바디 컴포넌트 가져오기
        bulletRigidbody.velocity = muzzlePoint.forward * bulletSpeed; // 총알의 속도 설정
    }
}
