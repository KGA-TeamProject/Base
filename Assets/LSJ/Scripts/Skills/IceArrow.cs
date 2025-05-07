using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrow : Skill
{
    [Header("Conponents")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] private float bulletSpeed; // �Ѿ��� ���ư��� ��

    [Header("Propertis")]
    [SerializeField] GameObject explosionEffectPrefab;

    public Stack<GameObject> returnPool; // ������Ʈ Ǯ�� ������ ���� -> ���� �������� ����

    //public float damage; // ��ų ������
    //public float range; // ��ų ��Ÿ�
    //public float cooldown; // ��ų ��Ÿ��

    public IceArrow()
    {
        SkillName = "���� ȭ��"; // ��ų �̸� ����
        SkillDescription = "�������� �� ȭ���� �߻�"; // ��ų ���� ����
    }

    void OnEnable()
    {
        // Ȱ��ȭ �ɶ����� ������ �����Բ�. OnEnable
        rigid.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse); // �Ѿ��� ���ư��� ���� �ش�. -> transform.forward�� �Ѿ��� ���ư��� ������ �ǹ��Ѵ�.
        StartCoroutine(ReturnPool(3f)); // �ڷ�ƾ�� �����Ѵ�. -> �Ѿ��� �߻�� ��, ���� �ð��� ������ �����Ѵ�.
    }

    private void Update()
    {
        if (rigid.velocity.magnitude > 2)
        {
            transform.forward = rigid.velocity; // ȭ���� �չ����� �ӵ� ��������
        }

        // ������ �������� -> ���ӸŴ������� ���¸� �޾Ƽ�.
        //if ()
        //{
        //    StartCoroutine(ReturnPool());
        //}
    }

    // �������� ����������ұ�?
    // �ð��� ��������.
    // �ε�������.
    private IEnumerator ReturnPool(float delay = 0f)
    {
        yield return new WaitForSeconds(delay); // 3�� �Ŀ� ����ȴ�. -> �Ѿ��� �߻�� ��, ���� �ð�(3��)�� ������ �����Ѵ�.
        rigid.velocity = Vector3.zero; // ȭ���� �ӵ��� 0���� �����. -> ȭ���� �ӵ� �ʱ�ȭ.
        gameObject.SetActive(false); // ȭ�� ��Ȱ��ȭ�Ѵ�. -> ȭ���� �߻�� ��, ���� �ð��� ������ �����Ѵ�.
        returnPool.Push(gameObject); // ������Ʈ Ǯ�� ����������.
    }

    private void OnTriggerEnter(Collider other)    // ���Ϳ� �浹�ϸ�, ����
    {
        if (other.gameObject.layer == 8) // ���Ϳ� �浹���� ��
        {
            //Destroy(gameObject); // �Ѿ��� �����Ѵ�.

            StartCoroutine(ReturnPool()); // ������Ʈ Ǯ�� �߰��Ѵ�. -> �Ѿ��� �߻�� ��, ���� �ð��� ������ �����Ѵ�.
        }
    }


    //private void OnCollisionEnter(Collision collision) // OnCollisionEnter -> �浹ü�� ������ ���۵� ��
    //{
    //    Destroy(gameObject);    // �����ϸ鼭
    //    // Destroys�� ���� ���� ����̶� �ð��� �Է°���
    //    // Destroyimmediate �� ��� ����. ������ ����.
    //    Instantiate(explosionEffectPrefab, transform.position, transform.rotation); // ���� ����Ʈ�� ��������

    //    //IDamagable damagable = collision.gameObject.GetComponent<IDamagable>(); // �������� ���� �� �ִ� ������Ʈ�� �ҷ���
    //    //if (damagable != null)  // �������� ���� �� �ִ� ������Ʈ�� null �� �ƴҶ�
    //    //{
    //    //    Debug.Log($"{collision.gameObject.name} ���� �Ѿ��� �������� ���� �� �ִ� ������Ʈ�� ������");
    //    //    Attack(damagable);  // ����. �������� �ش�.
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log($"{collision.gameObject.name} ���� �������� ���� �� �ִ� ������Ʈ�� ����");
    //    //}

    //    //Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();   // �� Rigidbody �־�?
    //    //if (rigidbody != null) // ���� Rigidbody �� ������
    //    //{
    //    //    rigidbody.AddForce(transform.forward * 10f, ForceMode.Impulse); // �������� 10f��ŭ �в�
    //    //}
    //}


}

