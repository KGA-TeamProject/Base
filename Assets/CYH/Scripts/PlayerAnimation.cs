using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    public PlayerMove2 playerMove;
    public PlayerAttack playerAttack;
    
    private void Update()
    {
        AnimationPlay();
    }

    public void AnimationPlay()
    {
        if (playerMove.isMove == false)
        {
            if (playerAttack.isDetect == true)
            {
                animator.Play("Attack1");
            }
            else
            {
                animator.Play("Idle1");
            }
        }
        else
        {
            animator.Play("Run");
        }
    }
   
}
