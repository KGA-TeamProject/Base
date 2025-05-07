using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] Shooter shooter; // �Ѿ� �߻��
    private void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            shooter.Fire();
            //GameManager.Instance.score++;   // static �� GameManager Instance �� ���������� �����ؼ� �򶧸��� 1��
        }
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    Inventory.Instance.UseItem(0); // �κ��丮���� ù ��° ������ ���
        //}
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(x, 0, z);
        if (dir.sqrMagnitude > 1f)
        {
            dir.Normalize();
        }
        transform.Translate(dir * Speed * Time.deltaTime);
    }
}
