using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Projectile prefab;
    public int shootCount;
    public float angle = 15;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // �߻� �Ѿ� ������ ���� �߻� ���� ����
        for (int i = 0; i < shootCount; i++)
        {
            float arrowAngle = -angle / 2 + angle / (shootCount + 1) * (i + 1);
            Quaternion arrowRotation = Quaternion.Euler(0, arrowAngle, 0);

            Projectile instance = Instantiate(prefab, transform.position, transform.rotation * arrowRotation);
            instance.rigid.velocity = instance.transform.forward * instance.speed;
        }
    }
}
