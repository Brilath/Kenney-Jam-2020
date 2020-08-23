using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D bodyCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private Vector2 jumpPower;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 desiredVelocity;
    [SerializeField] private bool desiredJump;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color cursedColor;
    [SerializeField] private float curseImmuneTime;
    [SerializeField] private float curseImmuneTimeCountDown;
    [SerializeField] private PhotonView playerPhotonview;
    public bool CanBeCursed;
    public bool IsCursed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        playerPhotonview = GetComponent<PhotonView>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentMoveSpeed = maxSpeed;
        IsCursed = false;
    }

    private void Update()
    {
        animator.SetFloat("speed", body.velocity.x);
        desiredVelocity.x = Input.GetAxis("Horizontal") * currentMoveSpeed;
        desiredJump |= Input.GetButtonDown("Jump");

        if (curseImmuneTimeCountDown > 0)
        {
            curseImmuneTimeCountDown -= Time.deltaTime;
        }
        else
        {
            CanBeCursed = true;
        }
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
        if (OnGround())
        {
            animator.SetTrigger("jump");
            velocity.y = jumpPower.y;
        }
    }

    private bool OnGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(bodyCollider.bounds.center,
                            bodyCollider.bounds.size, 0f,
                            Vector2.down, 0.1f, groundMask);
        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Triggered! {other.gameObject.name} with tag {other.gameObject.name}");
        if (other.gameObject.tag == "NPC")
        {
            CursePlayer();
        }
        if (other.gameObject.tag == "Player")
        {
            PlayerController controller = other.gameObject.GetComponent<PlayerController>();
            if (controller.IsCursed && CanBeCursed)
            {
                controller.UnCursePlayer();
                CursePlayer();
            }
            else if (!controller.IsCursed && controller.CanBeCursed)
            {
                controller.CursePlayer();
                UnCursePlayer();
            }
        }
    }

    public void CursePlayer()
    {
        IsCursed = true;
        CanBeCursed = true;
        spriteRenderer.color = cursedColor;
        StartCoroutine(SlowCursedPlayer());
    }
    public void UnCursePlayer()
    {
        IsCursed = false;
        CanBeCursed = false;
        spriteRenderer.color = normalColor;
        curseImmuneTimeCountDown = curseImmuneTime;
    }
    private IEnumerator SlowCursedPlayer()
    {
        Debug.Log($"Slowing Cursed Player!");
        float time = 2f;
        currentMoveSpeed = 0f;
        while (time >= 0)
        {
            time -= Time.deltaTime;
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, maxSpeed, time * Time.deltaTime);
            yield return null;
        }
        currentMoveSpeed = maxSpeed;
    }
}
