using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private float deActiveTime;
    private Rigidbody rigid;
    private float countTime;
    [SerializeField] public int attackDamage;
    
    


    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        AtActive();
    }
    private void Update()
    {
        TimeCount();
        transform.forward = rigid.velocity;

    }

    // ��Ȱ��ȭ �� �ӵ��� 0���� �ʱ�ȭ
    private void OnDisable()
    {
        rigid.velocity = Vector3.zero;
    }
    private void Init()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // ������ ���ÿ� ī��Ʈ �ٿ� ����
    private void TimeCount()
    {
        countTime -= Time.deltaTime;
        if (countTime <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void AtActive()
    {
        countTime = deActiveTime;

    }


}
