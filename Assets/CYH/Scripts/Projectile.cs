using UnityEngine;

public class Projectile : MonoBehaviour
{
    //�߻�ü�� �ʼ� �Ӽ� (��ų�� ���ϴ� �Ӽ�)
    public int damage;
    public int shootCount;
    public float speed;
    public float angle;
    public float delaySecond;
    public Rigidbody rigid;

    [SerializeField] private GameObject explosionEffect;

    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    // ��ų �����۰� ȭ�� �Ӽ� ����
    public void Init(int _damage, int _shootCount, float _speed, float _delaySecond, float _angle)
    {
        damage = _damage;
        shootCount = _shootCount;
        speed = _speed;
        delaySecond = _delaySecond;
        angle = _angle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);

        monsterController monsterController = collision.gameObject.GetComponent<monsterController>();
        if (monsterController != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
            monsterController.TakeDamage(damage);
        }
    }
}
