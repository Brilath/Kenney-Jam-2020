using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private Vector2 jumpPower;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 desiredVelocity;
    [SerializeField] private bool desiredJump;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        animator.SetFloat("speed", body.velocity.x);
        desiredVelocity.x = Input.GetAxis("Horizontal") * moveSpeed;
        desiredJump |= Input.GetButtonDown("Jump");
    }

    private void FixedUpdate()
    {
        float changeSpeed = accelerationSpeed * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, changeSpeed);
        velocity.y = Mathf.MoveTowards(velocity.y, jumpPower.y, changeSpeed);

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        else
        {
            velocity.y = body.velocity.y;
        }
        body.velocity = velocity;
    }

    private void Jump()
    {
        animator.SetTrigger("jump");
        if (body.velocity.y < 0.01 && body.velocity.y > -0.01)
        {
            velocity.y = jumpPower.y;
        }
        animator.ResetTrigger("jump");
    }
}
