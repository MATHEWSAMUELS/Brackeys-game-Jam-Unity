using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using Ilumisoft.HealthSystem;
using UnityEngine.SceneManagement; // Required for Scene Management

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Stats")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 15f;

    [Header("Jump Settings")]
    public int maxJumps = 1;

    [Header("Dash Stats")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Respawn")]
    public Transform spawnPoint;
    public float fallThreshold = -10f;

    // --- NEW: GAME OVER SETTINGS ---
    [Header("Game Over")]
    public string loseSceneName = "LoseScene"; // Type the name of your scene file here
    private bool isDead = false;    // To stop input when dead

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
    private GameObject pulseVisualEffect;
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

    [Header("Audio Settings")]
    public AudioSource playerAudioSource;
    public AudioClip jumpSound;
    public AudioClip shootSound;

    [Header("Movement Sound")]
    public AudioClip movementSound; 
    public float stepInterval = 0.5f; 
    private float nextStepTime = 0f;  
   

    private bool isGrounded;
    private float moveInput;
    private float verticalInput;
    private int jumpsLeft;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isFuture = false;
    private Ilumisoft.HealthSystem.Health playerHealth;

    void Start()
    {
        rb.gravityScale = pastGravity;

        playerHealth = GetComponent<Ilumisoft.HealthSystem.Health>();   
        playerHealth.OnHealthEmpty += OnPlayerDeath;
    }

    void Update()
    {
        if (isDead) return; 

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

        // GROUND CHECK
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.35f, groundLayer);

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

        if (isGrounded && moveInput != 0 && Time.time >= nextStepTime)
        {
            if (playerAudioSource != null && movementSound != null)
            {
                playerAudioSource.PlayOneShot(movementSound);
            }
            nextStepTime = Time.time + stepInterval;
        }
        
        // --- FALL CHECK (GAME OVER) ---
        if (transform.position.y < fallThreshold)
        {
            GameOver();
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

    // --- NEW GAME OVER LOGIC ---
    void GameOver()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero; 

        // 1. SAVE the current level name to memory (PlayerPrefs)
        PlayerPrefs.SetString("PreviousLevel", SceneManager.GetActiveScene().name);

        // 2. Load the Lose Screen
        if (!string.IsNullOrEmpty(loseSceneName))
        {
            SceneManager.LoadScene(loseSceneName);
        }
    }

    private void OnPlayerDeath()
    {
        // UPDATED: Load the lose screen scene instead of respawning
        GameOver();
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (playerAudioSource != null && jumpSound != null)
        {
            playerAudioSource.PlayOneShot(jumpSound);
        }
    }

    void Shoot()
    {
        Debug.Log("Shoot Method Started!");
        if (animator != null)
        {
            Debug.Log("Animator found, setting IsShooting to TRUE");
            animator.SetBool("IsShooting", true);
            StartCoroutine(ResetShootAnimationCoroutine());
        }
         else 
        {
             Debug.LogError("Animator is NULL!"); 
        }

        GameObject bullet = Instantiate(playerBulletPrefab, firePoint.position, firePoint.rotation);
        if (spriteHolder.localScale.x < 0)
        {
            bullet.transform.Rotate(0, 180, 0);
        }
        if (playerAudioSource != null && shootSound != null)
        {
            playerAudioSource.PlayOneShot(shootSound);
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

    // Respawn is kept here but is no longer called by death logic.
    public void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint.position;

        isFuture = false;
        pastLayout.SetActive(true);
        futureLayout.SetActive(false);
    }

    IEnumerator ResetShootAnimationCoroutine()
    {
        yield return new WaitForSeconds(0.2f); 
        if (animator != null)
        {
            animator.SetBool("IsShooting", false);
        }
    }
}