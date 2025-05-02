using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class nearAttack : MonoBehaviour
{
    [SerializeField] monsterController monsterController;
    [SerializeField] private Animator animator;

    [SerializeField] Transform targetPos;
    [SerializeField] LayerMask targetLayer;

    
    [SerializeField] private float rushSpeed;
    private Coroutine attackCoroutine;
    

    [SerializeField] private float attackRadius;
    

    
    private void Update()
    {
        detactPlayer();
    }

    private void OnDestroy()
    {
        if (attackCoroutine != null)
        {
            attackCoroutine = null;
        }
    }


    // attackRadius 안에 들어오면
    private void detactPlayer()
    {
        if (Physics.OverlapSphere(transform.position, attackRadius, targetLayer).Length > 0) 
        {
            
            Vector3 lookPos = new Vector3(targetPos.position.x, transform.position.y, targetPos.position.z);
            transform.LookAt(lookPos);

           
            if (attackCoroutine == null)
                attackCoroutine = StartCoroutine(Attack());
        }
        else
        {
            monsterController.Move(targetPos);
            
            
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    // 돌진하여 공격 충돌 데미지는 monsterController에서 구현
    private IEnumerator Attack()
    {
        
        while (true)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    animator.Play("Attack");
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetPos.position,
                        rushSpeed * Time.deltaTime);
                }
            }
            yield return null;
        }
        
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

    }

}
