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

    [Header("Respawn")]
    public Transform spawnPoint;
    public float fallThreshold = -10f;


    [Header("Layout References")]
    public GameObject pastLayout;
    public GameObject futureLayout;

    [Header("References")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform spriteHolder;

    private bool isGrounded;
    private float moveInput;
    private float verticalInput;
    private int jumpsLeft;
    private bool isDashing = false;
    private bool canDash = true;

    private bool isFuture = false;

    void Update()
    {
        if (isDashing) return;

        // SWITCH LAYOUT
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchLayout();
        }

        // GET INPUT
        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // INVERT CONTROLS IN FUTURE
        if (isFuture)
        {
            moveInput *= -1f;
        }

        // GROUND CHECK
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
        {
            jumpsLeft = maxJumps;
        }

        // JUMP
        if (Input.GetKeyDown(KeyCode.W) && jumpsLeft > 0)
        {
            Jump();
            jumpsLeft--;
        }

        // DASH
        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(PerformDash());
        }

        // FLIP SPRITE
        if (moveInput > 0)
        {
            if (spriteHolder != null)
                spriteHolder.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            if (spriteHolder != null)
                spriteHolder.localScale = new Vector3(-1, 1, 1);
        }

        //RESPAWM
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
            }

    void FixedUpdate()
    {
        if (isDashing) return;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector2 targetVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.15f);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    IEnumerator PerformDash()
    {
        // Only allow dash if pressing A or D
        if (Mathf.Abs(moveInput) < 0.1f)
            yield break;

        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        // Horizontal dash only
    float dashDirection = Mathf.Sign(spriteHolder.localScale.x);
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    void SwitchLayout()
    {
        isFuture = !isFuture;

        pastLayout.SetActive(!isFuture);
        futureLayout.SetActive(isFuture);
    }

    void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint.position;

        // Reset layout to Past when respawning (optional but recommended)
        isFuture = false;
        pastLayout.SetActive(true);
        futureLayout.SetActive(false);
    }


}
