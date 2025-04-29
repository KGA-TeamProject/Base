using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class farAttack : MonoBehaviour
{
    
    [SerializeField] Transform targetPos;
    [SerializeField] LayerMask targetLayer;
 
    [SerializeField] private float setDelay;
    private Coroutine attackCoroutine;
    private YieldInstruction attackDelay;

    [SerializeField] private float attackRadius;
    [SerializeField] monsterController monsterController;

    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private int poolSize;
    private GameObject[] attackPool;
    private Rigidbody attackRigid;
    [SerializeField] private float shotPower;

    [SerializeField] Transform attackPos;

    

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        attackDelay = new WaitForSeconds(setDelay);
    }

    private void Init()
    {
        attackPool = new GameObject[poolSize];
        for (int i = 0; i < attackPool.Length; i++)
        {
            attackPool[i] = Instantiate(attackPrefab);
            attackPool[i].SetActive(false);
        }
    }

    private void Update()
    {
        detactPlayer();
    }
    private void detactPlayer()
    {
        if (Physics.OverlapSphere(transform.position, attackRadius, targetLayer).Length > 0) // 오버랩스피어에 플레이어 레이어를 가진 콜라이더가 하나라도 있으면
        {
            // 총구가 플레이어를 바라봐야함.
            Vector3 lookPos = new Vector3(targetPos.position.x, transform.position.y, targetPos.position.z);
            transform.LookAt(lookPos);


            if (attackCoroutine == null)
                attackCoroutine = StartCoroutine(AttackCoroutine());
        }
        else // 아니면
        {
            monsterController.Move(targetPos);

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private void OnDestroy()
    {
        if (attackCoroutine != null)
        {
            attackCoroutine = null;
        }
    }


    private IEnumerator AttackCoroutine()
    {

        while (true)
        {

            Attack();
            yield return attackDelay;
        }

    }

    public void Attack()
    {
        foreach (GameObject bullet in attackPool)
        {
            if (!bullet.activeSelf)
            {
                bullet.transform.position = attackPos.position;
                bullet.transform.rotation = attackPos.rotation;
                attackRigid = bullet.GetComponent<Rigidbody>();
                bullet.SetActive(true);
                attackRigid.AddForce(attackPos.forward * shotPower, ForceMode.Impulse);

                return;
            }

        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

    }
}
