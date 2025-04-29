using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] Shooter shooter; // 총알 발사기
    private void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            shooter.Fire();
            //GameManager.Instance.score++;   // static 인 GameManager Instance 에 전역적으로 접근해서 쏠때마다 1점
        }
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    Inventory.Instance.UseItem(0); // 인벤토리에서 첫 번째 아이템 사용
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
