using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // ������ - ȭ�� �Ӽ� ���� ���2
    //public ArrowSkillData currentArrow;
    public Projectile prefab;
    public PlayerMove2 playerMove;
    public bool isDetect = false;
    private Coroutine shootCoroutine;

    [SerializeField] public float sightRange;       // �� Ž�� ����
    [SerializeField] private bool isMove = false;

    private void Update()
    {
        // �÷��̾ �������� ���� ���� ����
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

            // delaysecond ���� �߻� ex) delaysecond = 0.5f -> 2��/��
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
        Debug.Log("�߻�");

        // ȹ���� ��ų�� ȭ�� �Ӽ� ����
        prefab.Init(prefab.damage, prefab.shootCount, prefab.speed, prefab.delaySecond, prefab.angle);

        // �߻� ȭ�� ������ ���� �߻� ���� ����
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
            // �ش� ��ũ��Ʈ�� ���� ������Ʈ �Ǻ� �� ����
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
                // ������ ���Ͱ� ������ �߻� �ڷ�ƾ ����
                if (shootCoroutine != null)
                {
                    StopCoroutine(shootCoroutine);
                    shootCoroutine = null;
                }
            }
        }
    }
}
