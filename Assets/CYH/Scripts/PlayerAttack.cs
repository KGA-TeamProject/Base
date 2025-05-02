using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // 아이템 - 화살 속성 연결 방법2
    //public ArrowSkillData currentArrow;
    public Projectile prefab;
    public PlayerMove2 playerMove;
    public bool isDetect = false;
    private Coroutine shootCoroutine;

    [SerializeField] public float sightRange;       // 적 탐지 범위
    [SerializeField] private bool isMove = false;

    private void Update()
    {
        // 플레이어가 움직이지 않을 때만 공격
        if (isMove == false)
        {
            DetectMonster();
        }
    }

    IEnumerator ShootRoutine()
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;

            // delaysecond 마다 발사 ex) delaysecond = 0.5f -> 2번/초
            if (timer >= prefab.delaySecond)
            {
                Shoot();
                timer = 0f;
            }
            yield return null;
        }
    }

    public void Shoot()
    {
        Debug.Log("발사");

        // 획득한 스킬로 화살 속성 변경
        prefab.Init(prefab.damage, prefab.shootCount, prefab.speed, prefab.delaySecond, prefab.angle);

        // 발사 화살 개수에 따른 발사 각도 조절
        for (int i = 0; i < prefab.shootCount; i++)
        {
            float arrowAngle = -prefab.angle / 2 + prefab.angle / (prefab.shootCount + 1) * (i + 1);
            Quaternion arrowRotation = Quaternion.Euler(0, arrowAngle, 0);

            Projectile instance = Instantiate(prefab, transform.position, transform.rotation * arrowRotation);
            instance.rigid.velocity = instance.transform.forward * instance.speed;
        }
    }

    private void DetectMonster()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, sightRange))
        {
            // 해당 스크립트를 가진 오브젝트 판별 후 공격
            monsterController monsterController = hitInfo.collider.gameObject.GetComponent<monsterController>();
            if (monsterController != null)
            {
                isDetect = true;
                Debug.DrawLine(transform.position, hitInfo.point, Color.red);
                if (shootCoroutine == null)
                {
                    shootCoroutine = StartCoroutine(ShootRoutine());
                }
            }
            else
            {
                isDetect = false;
                // 범위에 몬스터가 없으면 발사 코루틴 삭제
                if (shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                    shootCoroutine = null;
                }
            }
        }
    }
}
