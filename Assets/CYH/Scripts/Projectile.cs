using UnityEngine;

public class Projectile : MonoBehaviour
{
    //발사체별 필수 속성
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
        
        // 충돌한 몬스터 체력 감소
        MonsterRayTest monsterRayTest = collision.gameObject.GetComponent<MonsterRayTest>();
        if (monsterRayTest != null)
        {
            collision.gameObject.GetComponent<MonsterRayTest>().hp -= damage;
            Debug.Log($"몬스터 체력 : {collision.gameObject.GetComponent<MonsterRayTest>().hp}");
        }
    }
}
