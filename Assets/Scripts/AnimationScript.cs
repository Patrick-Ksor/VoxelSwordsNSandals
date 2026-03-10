using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isBackWalkingHash;
    int isRightWalkingHash;
    int isLeftWalkingHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("walking");
        isBackWalkingHash = Animator.StringToHash("backwalk");
        isRightWalkingHash = Animator.StringToHash("rightwalk");
        isLeftWalkingHash = Animator.StringToHash("leftwalk");
    }

    // Update is called once per frame
    void Update()
    {

        bool isWalking = animator.GetBool(isWalkingHash);
        bool isBackWalk = animator.GetBool(isBackWalkingHash);
        bool isRightWalk = animator.GetBool(isRightWalkingHash);
        bool isLeftWalk = animator.GetBool(isLeftWalkingHash);
        bool forwardPressed = Input.GetKey("w");
        bool backPressed = Input.GetKey("s");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");

        if (!isWalking && forwardPressed)
        {
            animator.SetBool("walking", true);
        }
        if (isWalking && !forwardPressed)
        {
            animator.SetBool("walking", false);
        }
        if (!isBackWalk && backPressed)
        {
            animator.SetBool("backwalk", true);
        }
        if (isBackWalk && !backPressed)
        {
            animator.SetBool("backwalk", false);
        }
        if (!isRightWalk && rightPressed)
        {
            animator.SetBool("rightwalk", true);
        }
        if (isRightWalk && !rightPressed)
        {
            animator.SetBool("rightwalk", false);
        }
        if (!isLeftWalk && leftPressed)
        {
            animator.SetBool("leftwalk", true);
        }
        if (isLeftWalk && !leftPressed)
        {
            animator.SetBool("leftwalk", false);
        }
    }
}
