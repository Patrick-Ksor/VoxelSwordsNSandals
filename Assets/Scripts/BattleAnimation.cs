using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAnimation : MonoBehaviour

{
    Animator animator;
    int isAttackHash;
    public GameObject projectile;
    public Transform player;


    void Start()
    {
        animator = GetComponent<Animator>();
        isAttackHash = Animator.StringToHash("isAttack");
        player = GameObject.Find("Knight").transform;
    }

    // Update is called once per frame
    void Update()
    {

       
        bool isAttack = animator.GetBool(isAttackHash);
        bool leftClick = Input.GetMouseButtonDown(0);

        if (!isAttack && leftClick)
        {
            transform.LookAt(player);
            animator.SetBool("isAttack", true);
            StartCoroutine(Example());
            
        }
        if (isAttack && !leftClick)
        {
            animator.SetBool("isAttack", false);
        }
    }
    IEnumerator Example()
    {
        
        yield return new WaitForSeconds(1);
        Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 15f, ForceMode.Impulse);
        rb.AddForce(transform.up * 3f, ForceMode.Impulse);
    }
}
