using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D bodyCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PhotonView playerPhotonview;
    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float currentMoveSpeed;
    [SerializeField] private float accelerationSpeed;
    [SerializeField] private float airAccelerationSpeed;
    [SerializeField] private Vector2 jumpPower;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private Vector2 desiredVelocity;
    [SerializeField] private bool desiredJump;
    [SerializeField] private LayerMask groundMask;
    [Header("Curse")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color cursedColor;
    [SerializeField] private float curseImmuneTime;
    [SerializeField] private float curseImmuneTimeCountDown;

    public bool CanBeCursed;
    public bool IsCursed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        playerPhotonview = GetComponent<PhotonView>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentMoveSpeed = maxSpeed;
    }
    private void Start()
    {
        bool isCursed = false;
        bool canBeCursed = true;
        playerPhotonview.RPC("RPCSetStatus", RpcTarget.AllBuffered, isCursed, canBeCursed);
    }

    private void Update()
    {
        animator.SetFloat("speed", Math.Abs(body.velocity.x));
        desiredVelocity.x = Input.GetAxis("Horizontal") * currentMoveSpeed;
        desiredJump |= Input.GetButtonDown("Jump");

        if (curseImmuneTimeCountDown > 0)
        {
            curseImmuneTimeCountDown -= Time.deltaTime;
        }
        else
        {
            Debug.Log($"Not immune to curse {PhotonNetwork.LocalPlayer.UserId}");
            CanBeCursed = true;
            playerPhotonview.RPC("RPCSetStatus", RpcTarget.AllBuffered, IsCursed, CanBeCursed);
        }
    }

    private void FixedUpdate()
    {
        float acceleration = OnGround() ? accelerationSpeed : airAccelerationSpeed;
        float changeSpeed = acceleration * Time.deltaTime;
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
            // If other player is cursed and you are not
            if (controller.IsCursed && CanBeCursed)
            {
                controller.UnCursePlayer();
                CursePlayer();
            }
            // If other player is not cursed and can be cursed
            else if (!controller.IsCursed && controller.CanBeCursed && IsCursed)
            {
                controller.CursePlayer();
                UnCursePlayer();
            }
        }
    }
    public void CursePlayer()
    {
        bool isCursed = true;
        bool canBeCursed = false;
        IsCursed = isCursed;
        CanBeCursed = canBeCursed;
        playerPhotonview.RPC("RPCSetStatus", RpcTarget.AllBuffered, isCursed, canBeCursed);
        playerPhotonview.RPC("RPCSetSpriteColor", RpcTarget.AllBuffered, cursedColor.r, cursedColor.g, cursedColor.b);
        StartCoroutine(SlowCursedPlayer());
    }
    public void UnCursePlayer()
    {
        bool isCursed = false;
        bool canBeCursed = true;
        IsCursed = isCursed;
        CanBeCursed = canBeCursed;
        playerPhotonview.RPC("RPCSetStatus", RpcTarget.AllBuffered, isCursed, canBeCursed);
        playerPhotonview.RPC("RPCSetSpriteColor", RpcTarget.AllBuffered, normalColor.r, normalColor.g, normalColor.b);
        curseImmuneTimeCountDown = curseImmuneTime;
    }
    [PunRPC]
    private void RPCSetStatus(bool isCursed, bool canBeCursed)
    {
        IsCursed = isCursed;
        CanBeCursed = canBeCursed;
    }
    [PunRPC]
    private void RPCSetSpriteColor(float r, float g, float b)
    {
        spriteRenderer.color = new Color(r, g, b);
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
