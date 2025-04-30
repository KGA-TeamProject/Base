using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab; // �Ѿ� ������
    [SerializeField] Transform muzzlePoint; // �Ѿ� �߻� ��ġ

    [Range(15, 40)]
    [SerializeField] float bulletSpeed; // �߻� �ӵ�


    public void Fire()
    {
        // �Ѿ��� �߻��ϴ� �޼���
        GameObject instance = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation); // �Ѿ� �ν��Ͻ� ����
                                                                                                     // ���� ����Ʈ ��ġ�� ȸ������ �Ѿ� ����
        Destroy(instance, 3); // 3�� �� �Ѿ� ����

        Rigidbody bulletRigidbody = instance.GetComponent<Rigidbody>(); // �Ѿ��� ������ٵ� ������Ʈ ��������
        bulletRigidbody.velocity = muzzlePoint.forward * bulletSpeed; // �Ѿ��� �ӵ� ����
    }
}
