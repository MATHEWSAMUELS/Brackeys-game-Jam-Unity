using UnityEngine;
using System.Collections;

public class EnemyMelee : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;

    [Header("Detection")]
    public float detectionRange = 6f;
    public float attackRange = 1.5f; // How close to get before attacking
    public float chaseSpeed = 4f;     // How fast to run at the player

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float attackDuration = 0.3f; // How long the attack animation lasts
    public int attackDamage = 10;      // Damage to deal (if you have a health script)

    [Header("References")]
    public Animator animator;
    public Transform spriteHolder;

    private Transform player;
    private Rigidbody2D rb;

    private bool movingToB = true;
    private bool isAlerted = false;
    private bool canAttack = true;
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

        if (distanceToPlayer <= detectionRange)
            isAlerted = true;
        else
            isAlerted = false;

        if (isAlerted)
            HandleChase(distanceToPlayer);
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

    void HandleChase(float distanceToPlayer)
    {
        // If we are far away, run towards the player
        if (distanceToPlayer > attackRange)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);

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
            // We are close enough to attack. Stop moving.
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

            if (canAttack)
                StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
        {
            canAttack = false;

            if (animator != null) animator.SetBool("IsAttacking", true);

            // --- DEAL DAMAGE USING NEW HEALTH SYSTEM ---
            Ilumisoft.HealthSystem.Health playerHealth = 
                player.GetComponent<Ilumisoft.HealthSystem.Health>();

            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(attackDamage);
                Debug.Log("Enemy Melee attacked player for " + attackDamage);
            }

            yield return new WaitForSeconds(attackDuration);

            if (animator != null) animator.SetBool("IsAttacking", false);

            yield return new WaitForSeconds(attackCooldown - attackDuration);

            canAttack = true;
        }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if player landed on top (Mario style kill)
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

        // Disable attacking and patrol
        StopAllCoroutines();
    }
}