using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(Animator))]
public class AddWindToSails : MonoBehaviour
{
    private Animation anim;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("SailsDown", true);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            animator.SetBool("SailsDown", false);
        }
    }
}
