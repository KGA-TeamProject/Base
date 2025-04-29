using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Projectile prefab;

    public int shootCount;
    public float angle = 15;
    
    [SerializeField] public float sightRange;
    [SerializeField] private bool isMove;

    private void Update()
    {
        DetectMonster();
    }

    public void Shoot()
    {
        // 발사 총알 개수에 따른 발사 각도 조절
        for (int i = 0; i < shootCount; i++)
        {
            float arrowAngle = -angle / 2 + angle / (shootCount + 1) * (i + 1);
            Quaternion arrowRotation = Quaternion.Euler(0, arrowAngle, 0);

            // 화살 
            Projectile instance = Instantiate(prefab, transform.position, transform.rotation * arrowRotation);
            instance.rigid.velocity = instance.transform.forward * instance.speed;
        }
    }

    private void DetectMonster()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, sightRange))
        {
            // 해당 스크립트를 가진 오브젝트 판별 후 공격
            MonsterRayTest monsterRayTest = hitInfo.collider.gameObject.GetComponent<MonsterRayTest>();
            
            if (monsterRayTest != null)
            {
                Debug.DrawLine(transform.position, hitInfo.point, Color.green);
                Debug.Log($" 맞음 : {hitInfo.collider.gameObject.name}");
                Shoot();
            }
        }
    }
}
