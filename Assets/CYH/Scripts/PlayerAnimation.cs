using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    public PlayerMove2 playerMove;

    private void Update()
    {
        AnimationPlay();
    }

    public void AnimationPlay()
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
