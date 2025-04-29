using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nearAttack : MonoBehaviour
{
   
    [SerializeField] Transform targetPos;
    [SerializeField] LayerMask targetLayer;

    
    [SerializeField] private float rushSpeed;
    private Coroutine attackCoroutine;
    

    [SerializeField] private float attackRadius;
    [SerializeField] monsterController monsterController;

    
    private void Update()
    {
        detactPlayer();
    }
    private void detactPlayer()
    {
        if (Physics.OverlapSphere(transform.position, attackRadius, targetLayer).Length > 0) // ���������Ǿ �÷��̾� ���̾ ���� �ݶ��̴��� �ϳ��� ������
        {
            // �ѱ��� �÷��̾ �ٶ������.
            Vector3 lookPos = new Vector3(targetPos.position.x, transform.position.y, targetPos.position.z);
            transform.LookAt(lookPos);

           
            if (attackCoroutine == null)
                attackCoroutine = StartCoroutine(Attack());
        }
        else // �ƴϸ�
        {
            monsterController.Move(targetPos); 
            
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator Attack()
    {
        
        while (true)
        {
            
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos.position,
                rushSpeed * Time.deltaTime);
            yield return null;
        }
        
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

    }

}
