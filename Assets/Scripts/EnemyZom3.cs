using UnityEngine;
using System.Collections;

public class EnemyThrower : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 12f; // Spots player from far away
    public float throwRange = 2f;      // Stops walking at this distance
    public float chaseSpeed = 4f;      // How fast he walks towards you

    [Header("Throwing Settings")]
    public GameObject projectilePrefab; 
    public Transform firePoint;         
    public float throwCooldown = 2.5f;
    public float throwDuration = 0.5f;  
    public float throwDelay = 0.3f;     

    [Header("References")]
    public Animator animator;
    public Transform spriteHolder;

    private Transform player;
    private Rigidbody2D rb;

    private bool isAlerted = false;
    private bool canThrow = true;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Detection Check
        if (distanceToPlayer <= detectionRange)
            isAlerted = true;
        else
            isAlerted = false;

        if (isAlerted)
            HandleChase(distanceToPlayer);
        else
            StayIdle();
    }

    void StayIdle()
    {
        rb.linearVelocity = Vector2.zero; // Stop moving completely
        if (animator != null) animator.SetBool("IsWalking", false);
    }

    void HandleChase(float distanceToPlayer)
    {
        // If we are further than 2 units, walk towards player
        if (distanceToPlayer > throwRange)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);

            // --- WALK ANIMATION & FLIP ---
            if (animator != null) animator.SetBool("IsWalking", true);
            if (spriteHolder != null)
            {
                if (direction > 0)
                    spriteHolder.localScale = new Vector3(1, 1, 1);
                else
                    spriteHolder.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            // We are close enough (2 units). Stop and throw.
            rb.linearVelocity = Vector2.zero; // <--- FIX: Added stop logic

            // --- IDLE ANIMATION ---
            if (animator != null) animator.SetBool("IsWalking", false);

            // Keep facing the player
            if (spriteHolder != null)
            {
                float direction = Mathf.Sign(player.position.x - transform.position.x);
                if (direction > 0)
                    spriteHolder.localScale = new Vector3(1, 1, 1);
                else
                    spriteHolder.localScale = new Vector3(-1, 1, 1);
            }

            if (canThrow)
                StartCoroutine(ThrowProjectile());
        }
    }

    IEnumerator ThrowProjectile()
    {
        canThrow = false;

        // 1. Play Throw Animation
        if (animator != null) animator.SetBool("IsThrowing", true);

        // 2. Wait for the "Release" frame
        yield return new WaitForSeconds(throwDelay);

        // 3. Spawn Projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDirection = (player.position - firePoint.position).normalized;
        
        // <--- FIX: Using EnemyGrenade instead of EnemyProjectile
        projectile.GetComponent<EnemyGrenade>().Launch(shootDirection);

        // 4. Wait for the rest of the Throw Animation
        yield return new WaitForSeconds(throwDuration - throwDelay);

        // 5. Turn off Throw Animation
        if (animator != null) animator.SetBool("IsThrowing", false);

        // 6. Wait for Cooldown
        yield return new WaitForSeconds(throwCooldown);

        canThrow = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                Die();
            }
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 3f;
        if (animator != null) animator.SetBool("IsWalking", false);
        StopAllCoroutines();
    }
}