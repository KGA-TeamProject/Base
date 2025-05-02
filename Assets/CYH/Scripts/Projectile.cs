using UnityEngine;

public class Projectile : MonoBehaviour
{
    //발사체별 필수 속성 (스킬별 변하는 속성)
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

    // 스킬 아이템과 화살 속성 연결
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

        // 충돌한 몬스터 체력 감소
        monsterController monsterController = collision.gameObject.GetComponent<monsterController>();
        if (monsterController != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
            // merge 후 적용
            //monsterController.TakeDamage(damage);
            //Debug.Log($"몬스터 체력 : {collision.gameObject.GetComponent<monsterController>().Hp}");
        }
    }
}
