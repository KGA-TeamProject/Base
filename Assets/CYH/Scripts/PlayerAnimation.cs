using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    public PlayerMove2_Test playerMove;
    
    private void Update()
    {
        Walk();
    }

    public void Walk()
    {
        if (playerMove.isMove == false)
        {
            animator.Play("Idle1");
        }
        else
        {
            animator.Play("Run");
        }
    }
}
