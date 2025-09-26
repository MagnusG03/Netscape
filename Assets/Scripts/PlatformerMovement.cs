using System;
using UnityEngine;

public class PlatformerMovement : MonoBehaviour
{
    private float moveSpeed = 4f;
    private float crouchSpeed = 2f;
    private float walkSpeed = 4f;
    private float runSpeed = 6f;
    private float jumpForce = 8f;
    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    public Transform roofCheckLeft;
    public Transform roofCheckRight;
    private BoxCollider2D collider;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private float groundCheckRadius = 0.2f;
    private float roofCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool isHeadColliding;
    private bool facingRight = false;
    private Vector2 standingColliderSize;
    private Vector2 standingColliderOffset;

    float inputX;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
        standingColliderSize = collider.size;
        standingColliderOffset = collider.offset;
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(rigidbody.linearVelocity.x));
        animator.SetFloat("RunAnimationSpeed", Mathf.Abs(rigidbody.linearVelocity.x) / moveSpeed);

        // Character direction based on movement direction
        if (facingRight && inputX < 0)
        {
            facingRight = false;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (!facingRight && inputX > 0)
        {
            facingRight = true;
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.linearVelocity = new Vector2(rigidbody.linearVelocity.x, jumpForce);
        }

        // is on Ground and is on Roof Check
        isGrounded = Physics2D.OverlapCircle(groundCheckLeft.position, groundCheckRadius, groundLayer) || Physics2D.OverlapCircle(groundCheckRight.position, groundCheckRadius, groundLayer);
        isHeadColliding = Physics2D.OverlapCircle(roofCheckLeft.position, roofCheckRadius, groundLayer) || Physics2D.OverlapCircle(roofCheckRight.position, roofCheckRadius, groundLayer);
        if (isGrounded)
        {
            animator.SetBool("OnGround", true);
        }
        else
        {
            animator.SetBool("OnGround", false);
        }

        // Crouching
        if (isGrounded && Input.GetKey(KeyCode.LeftControl))
        {
            animator.SetBool("Crouching", true);
            moveSpeed = crouchSpeed;
            collider.size = new Vector2(standingColliderSize.x, standingColliderSize.y * 2 / 3);
            collider.offset = new Vector2(standingColliderOffset.x, standingColliderOffset.y - standingColliderSize.y / 6);
        }
        else if ((moveSpeed == crouchSpeed && !Input.GetKey(KeyCode.LeftControl) && !isHeadColliding) || (!isGrounded && !isHeadColliding && moveSpeed != runSpeed))
        {
            animator.SetBool("Crouching", false);
            moveSpeed = walkSpeed;
            collider.size = standingColliderSize;
            collider.offset = standingColliderOffset;
        }

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && !animator.GetBool("Crouching") && isGrounded)
        {
            moveSpeed = runSpeed;
        }
        else if (!animator.GetBool("Crouching") && Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = walkSpeed;
        }
    }

    void FixedUpdate()
    {
        // Move the character
        rigidbody.linearVelocity = new Vector2(inputX * moveSpeed, rigidbody.linearVelocity.y);
    }
}