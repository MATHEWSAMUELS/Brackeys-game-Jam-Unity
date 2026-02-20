using UnityEngine;
using System.Collections;

public class EnemyGuard : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;

    [Header("Detection")]
    public float detectionRange = 6f;
    public float stopDistance = 3f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;
    public float recoilTime = 0.2f; // <--- NEW: How long the shoot animation lasts

    [Header("References")]
    public Animator animator;
    public Transform spriteHolder;

    private Transform player;
    private Rigidbody2D rb;

    private bool movingToB = true;
    private bool isAlerted = false;
    private bool canShoot = true;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        if (distanceToPlayer <= detectionRange)
            isAlerted = true;
        else
            isAlerted = false;

        if (isAlerted)
            HandleAlert(distanceToPlayer);
        else
            Patrol();
    }

    void Patrol()
    {
        Transform target = movingToB ? pointB : pointA;

        float direction = Mathf.Sign(target.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);

        // --- ANIMATION & FLIP LOGIC ---
        if (animator != null) animator.SetBool("IsWalking", true);
        if (spriteHolder != null)
        {
            if (direction > 0)
                spriteHolder.localScale = new Vector3(1, 1, 1);
            else
                spriteHolder.localScale = new Vector3(-1, 1, 1);
        }

        if (Mathf.Abs(transform.position.x - target.position.x) < 0.2f)
        {
            movingToB = !movingToB;
        }
    }

    void HandleAlert(float distanceToPlayer)
    {
        if (distanceToPlayer > stopDistance)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);

            // --- ANIMATION & FLIP LOGIC ---
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
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // --- STOPPED ANIMATION ---
            if (animator != null) animator.SetBool("IsWalking", false);

            // Make sure the enemy still faces the player even when stopped
            if (spriteHolder != null)
            {
                float direction = Mathf.Sign(player.position.x - transform.position.x);
                if (direction > 0)
                    spriteHolder.localScale = new Vector3(1, 1, 1);
                else
                    spriteHolder.localScale = new Vector3(-1, 1, 1);
            }

            if (canShoot)
                StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        // 1. Play Animation
        if (animator != null) animator.SetBool("IsShooting", true);

        // 2. Spawn Bullet
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDirection = (player.position - firePoint.position).normalized;
        bullet.GetComponent<EnemyProjectile>().SetDirection(shootDirection);

        // 3. Wait for Recoil Animation to finish
        yield return new WaitForSeconds(recoilTime);

        // 4. Turn off Animation
        if (animator != null) animator.SetBool("IsShooting", false);

        // 5. Wait for the rest of the cooldown
        yield return new WaitForSeconds(shootCooldown - recoilTime);

        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if player landed on top
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

        // Let gravity pull it down
        rb.gravityScale = 3f;

        // Stop walking animation
        if (animator != null) animator.SetBool("IsWalking", false);

        // Disable shooting and patrol
        StopAllCoroutines();
    }
}