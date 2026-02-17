using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Stats")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 15f;
    
    [Header("Jump Settings")]
    public int maxJumps = 2; 

    [Header("Dash Stats")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("References")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Visuals")]
    public Transform spriteHolder;

    private bool isGrounded;
    private float moveInput;
    private float verticalInput;
    private int jumpsLeft;
    private bool isDashing = false; 

    void Update()
    {
        if (isDashing) return;

        moveInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
        {
            jumpsLeft = maxJumps;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (jumpsLeft > 0)
            {
                Jump();
                jumpsLeft--;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && canDash) StartCoroutine(PerformDash());

        // --- FLIPPING CHARACTER ---
        if (moveInput > 0)
        {
            if (spriteHolder != null) spriteHolder.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            if (spriteHolder != null) spriteHolder.localScale = new Vector3(-1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        // --- NORMAL MOVEMENT ---
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector2 targetVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.15f);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private bool canDash = true;
    IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; 

        Vector2 dashDirection = new Vector2(moveInput, verticalInput);
        if (dashDirection == Vector2.zero) dashDirection = new Vector3(transform.localScale.x, 0);
        
        rb.linearVelocity = dashDirection.normalized * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}