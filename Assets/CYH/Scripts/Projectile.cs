using UnityEngine;

public class Projectile : MonoBehaviour
{
    //�߻�ü�� �ʼ� �Ӽ�
    public int speed;
    public Rigidbody rigid;

    [SerializeField] private int damage;

    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        
        // �浹�� ���� ü�� ����
        MonsterRayTest monsterRayTest = collision.gameObject.GetComponent<MonsterRayTest>();
        if (monsterRayTest != null)
        {
            collision.gameObject.GetComponent<MonsterRayTest>().hp -= damage;
            Debug.Log($"���� ü�� : {collision.gameObject.GetComponent<MonsterRayTest>().hp}");
        }
    }
}
