using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class farAttack : MonoBehaviour
{
    [SerializeField] monsterController monsterController;
    [SerializeField] private Animator animator;

    //[SerializeField] Transform targetPos;
    [SerializeField] LayerMask targetLayer;
 
    [SerializeField] private float setDelay;
    private Coroutine attackCoroutine;
    private YieldInstruction attackDelay;

    [SerializeField] private float attackRadius;

    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private int poolSize;
    private GameObject[] attackPool;
    private Rigidbody attackRigid;
    [SerializeField] private int attackDamage;

    [SerializeField] private float shotPower;

    

    

    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        attackDelay = new WaitForSeconds(setDelay);
    }

    // 오브젝트 풀 생성
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

    public int GiveDamage()
    {
        return attackDamage;
    }

    // attackRadius 안에 들어오면
    private void detactPlayer()
    {
        if (Physics.OverlapSphere(transform.position, attackRadius, targetLayer).Length > 0)
        {
            Transform targetPos = Physics.OverlapSphere(transform.position, attackRadius, targetLayer)[0].transform;
            Vector3 lookPos = new Vector3(targetPos.position.x, transform.position.y, targetPos.position.z);
            transform.LookAt(lookPos);


            if (attackCoroutine == null)
            { 
                attackCoroutine = StartCoroutine(AttackCoroutine());
                Debug.Log("Attack Start");   
            }
        }
        else
        {
            monsterController.Move();

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

    // attackDelay 만큼의 간격으로
    private IEnumerator AttackCoroutine()
    {

        while (true)
        {
            Attack();
            animator.Play("Attack");
            yield return attackDelay;
        }

    }

    // 원거리 공격
    public void Attack()
    {
        foreach (GameObject bullet in attackPool)
        {
            if (!bullet.activeSelf)
            {
                bullet.transform.position = gameObject.transform.position;
                bullet.transform.rotation = gameObject.transform.rotation;
                attackRigid = bullet.GetComponent<Rigidbody>();
                bullet.SetActive(true);
                attackRigid.AddForce(gameObject.transform.forward * shotPower, ForceMode.Impulse);

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
