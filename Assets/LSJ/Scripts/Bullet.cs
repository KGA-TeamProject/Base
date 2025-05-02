using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Conponents")]
    [SerializeField] Rigidbody rigid;

    [Header("Propertis")]
    [SerializeField] GameObject explosionEffectPrefab;

    //public int attackPoint;

    //private void Awake()
    //{
    //    rigid = GetComponent<Rigidbody>();
    //}

    private void Update()
    {
        if (rigid.velocity.magnitude > 2)
        {
            transform.forward = rigid.velocity; // �Ҹ��� �չ����� �ӵ� ��������
        }

    }

    private void OnCollisionEnter(Collision collision) // OnCollisionEnter -> �浹ü�� ������ ���۵� ��
    {
        Destroy(gameObject);    // �����ϸ鼭
        // Destroys�� ���� ���� ����̶� �ð��� �Է°���
        // Destroyimmediate �� ��� ����. ������ ����.
        Instantiate(explosionEffectPrefab, transform.position, transform.rotation); // ���� ����Ʈ�� ��������

        //IDamagable damagable = collision.gameObject.GetComponent<IDamagable>(); // �������� ���� �� �ִ� ������Ʈ�� �ҷ���
        //if (damagable != null)  // �������� ���� �� �ִ� ������Ʈ�� null �� �ƴҶ�
        //{
        //    Debug.Log($"{collision.gameObject.name} ���� �Ѿ��� �������� ���� �� �ִ� ������Ʈ�� ������");
        //    Attack(damagable);  // ����. �������� �ش�.
        //}
        //else
        //{
        //    Debug.Log($"{collision.gameObject.name} ���� �������� ���� �� �ִ� ������Ʈ�� ����");
        //}

        //Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();   // �� Rigidbody �־�?
        //if (rigidbody != null) // ���� Rigidbody �� ������
        //{
        //    rigidbody.AddForce(transform.forward * 10f, ForceMode.Impulse); // �������� 10f��ŭ �в�
        //}
    }

    //private void Attack(IDamagable damagable)   // ���Ϳ� IDamagable �� �ֱ� ������ damagable -> Monster �� ��. 
    //{
    //    // TODO : 
    //    damagable.TakeDamage(gameObject, attackPoint);  // ������ TakeDamage�� ���� ��
    //}
}
