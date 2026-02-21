using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

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
    public Animator animator;

    [Header("Past Settings")]
    public float pastWalkSpeed = 4f;
    public float pastRunSpeed = 7f;
    public float pastGravity = 4f;

    [Header("Future Settings")]
    public float futureWalkSpeed = 7f;
    public float futureRunSpeed = 12f;
    public float futureGravity = 1.5f;

    // --- SHOOTING VARIABLES ---
    [Header("Shooting Settings")]
    public GameObject playerBulletPrefab; 
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    
    [Header("Grenade Settings")]
    public GameObject grenadePrefab;
    public float throwForce = 10f;
    public float upwardForce = 5f;
    public float grenadeCooldown = 2f;
    private float nextGrenadeTime = 0f;

    
    [Header("Pulse Ability Settings")]
    public GameObject pulseVisualEffect;
    public float pulseCooldown = 180f;
    public float pulseRadius = 8f;
    public float pulseForce = 25f;
    public float pulseDamage = 50f;
    private float nextPulseTime = 0f;

  
    [Header("Time Warp Ability (R Key)")]
    public float warpDuration = 5f;      
    public float warpCooldown = 20f;    
    public float deadEyeSpeedMultiplier = 2f; 
    public float rageEnemySpeedMultiplier = 2f; 
    private float nextWarpTime = 0f;
    private bool isWarpActive = false;
   

    private bool isGrounded;
    private float moveInput;
    private float verticalInput;
    private int jumpsLeft;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isFuture = false;


    void Start()
    {
        rb.gravityScale = pastGravity;
    }

    void Update()
    {
        if (isDashing) return;

        // SWITCH LAYOUT
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchLayout();
        }

        // --- SHOOTING INPUT ---
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // --- GRENADE INPUT ---
        if (Input.GetMouseButtonDown(1) && Time.time >= nextGrenadeTime)
        {
            ThrowGrenade();
            nextGrenadeTime = Time.time + grenadeCooldown;
        }

        // --- PULSE ABILITY INPUT ---
        if (Input.GetKeyDown(KeyCode.T) && Time.time >= nextPulseTime)
        {
            ActivatePulse();
            nextPulseTime = Time.time + pulseCooldown;
        }

        // --- TIME WARP INPUT (R Key) ---
        if (Input.GetKeyDown(KeyCode.R) && Time.time >= nextWarpTime && !isWarpActive)
        {
            StartCoroutine(ActivateTimeWarp());
        }

        // GET INPUT
        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // INVERT CONTROLS IN FUTURE
        // if (isFuture)
        // {
        //     moveInput *= -1f;
        // }

        // GROUND CHECK
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // --- ANIMATION LOGIC ---
        if (animator != null)
        {
            animator.SetBool("IsWalking", moveInput != 0);
            animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift));
            
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
            animator.SetBool("IsFalling", rb.linearVelocity.y < -0.1f && !isGrounded);
        }

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

        
        float currentWalkSpeed = isFuture ? futureWalkSpeed : pastWalkSpeed;
        float currentRunSpeed = isFuture ? futureRunSpeed : pastRunSpeed;

       
        if (isFuture && isWarpActive)
        {
            currentWalkSpeed *= deadEyeSpeedMultiplier;
            currentRunSpeed *= deadEyeSpeedMultiplier;
        }

        float speedToUse = Input.GetKey(KeyCode.LeftShift) ? currentRunSpeed : currentWalkSpeed;

        Vector2 targetVelocity = new Vector2(moveInput * speedToUse, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.15f);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void Shoot()
    {
        Debug.Log("Shoot Method Started!");
        // --- ANIMATION TRIGGER ---
        if (animator != null)
        {
            Debug.Log("Animator found, setting IsShooting to TRUE");
            animator.SetBool("IsShooting", true);
            StartCoroutine(ResetShootAnimationCoroutine());
        }
         else 
        {
             Debug.LogError("Animator is NULL!"); // <--- If this appears, check the Inspector slot
        }


        // --- BULLET LOGIC ---
        GameObject bullet = Instantiate(playerBulletPrefab, firePoint.position, firePoint.rotation);
        if (spriteHolder.localScale.x < 0)
        {
            bullet.transform.Rotate(0, 180, 0);
        }
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D grenadeRb = grenade.GetComponent<Rigidbody2D>();

        if (grenadeRb != null)
        {
            float direction = spriteHolder.localScale.x;
            Vector2 throwVector = new Vector2(direction * throwForce, upwardForce);
            grenadeRb.AddForce(throwVector, ForceMode2D.Impulse);

            Vector3 originalScale = grenade.transform.localScale;
            if (direction < 0)
            {
                grenade.transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            }
        }
    }

    
    void ActivatePulse()
    {
        float directionMultiplier = isFuture ? -1f : 1f;

        Debug.Log(directionMultiplier > 0 ? "Pulse Activated: PUSH!" : "Pulse Activated: PULL!");

        if (pulseVisualEffect != null)
        {
            Instantiate(pulseVisualEffect, transform.position, Quaternion.identity);
        }

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, pulseRadius);

        foreach (Collider2D enemy in enemiesInRange)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(pulseDamage);
                }

                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 direction = (enemy.transform.position - transform.position).normalized;
                    enemyRb.AddForce(direction * pulseForce * directionMultiplier, ForceMode2D.Impulse);
                }
            }
        }
    }

    
    IEnumerator ActivateTimeWarp()
    {
        isWarpActive = true;
        Debug.Log("Time Warp Started!");

       
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        
        Dictionary<EnemyGuard, float> enemyOriginalSpeeds = new Dictionary<EnemyGuard, float>();

        if (!isFuture)
        {
            
            foreach (GameObject enemyObj in allEnemies)
            {
               
                EnemyGuard enemyScript = enemyObj.GetComponent<EnemyGuard>();
                if (enemyScript != null)
                {
                    
                    float originalSpeed = enemyScript.patrolSpeed;
                    enemyOriginalSpeeds.Add(enemyScript, originalSpeed);
                    
                    enemyScript.patrolSpeed *= rageEnemySpeedMultiplier;
                }
            }
        }
        else
        {
            
            foreach (GameObject enemyObj in allEnemies)
            {
                EnemyGuard enemyScript = enemyObj.GetComponent<EnemyGuard>();
                if (enemyScript != null)
                {
                    float originalSpeed = enemyScript.patrolSpeed;
                    enemyOriginalSpeeds.Add(enemyScript, originalSpeed);
                    
                   
                    enemyScript.patrolSpeed /= rageEnemySpeedMultiplier; 
                }
            }
        }

        
        yield return new WaitForSeconds(warpDuration);

        
        isWarpActive = false;
        Debug.Log("Time Warp Ended.");

        
        foreach (var pair in enemyOriginalSpeeds)
        {
            if (pair.Key != null)
            {
                pair.Key.patrolSpeed = pair.Value;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pulseRadius);
    }

    IEnumerator PerformDash()
    {
        if (Mathf.Abs(moveInput) < 0.1f)
            yield break;

        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

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

        rb.gravityScale = isFuture ? futureGravity : pastGravity;
    }

    public void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint.position;

        isFuture = false;
        pastLayout.SetActive(true);
        futureLayout.SetActive(false);
    }

    // Helper to reset the shoot animation
    IEnumerator ResetShootAnimationCoroutine()
    {
        yield return new WaitForSeconds(0.2f); // Adjust this time to match your animation length
        if (animator != null)
        {
            animator.SetBool("IsShooting", false);
        }
    }
}