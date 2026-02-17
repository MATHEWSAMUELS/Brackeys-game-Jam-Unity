using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 3f;

    [Header("Chase Settings")]
    public float chaseRange = 5f;
    public float stopDistance = 1f;
    public float chaseSpeed = 6f;

    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    // Internal State
    private bool movingToA = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // --- STATE MACHINE ---
        
        // 1. CHASE (If player is close, but not too close)
        if (distanceToPlayer < chaseRange && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
        }
        // 2. STOP (If player is very close)
        else if (distanceToPlayer <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
        }
        // 3. PATROL (Default behavior)
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        // Flip to face player
        if (transform.position.x > player.position.x)
            sprite.flipX = true;
        else
            sprite.flipX = false;

        // Move towards player
        Vector2 targetPos = new Vector2(player.position.x, rb.linearVelocity.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);
    }

    void Patrol()
    {
        // Decide which point to walk to
        Transform targetPoint = movingToA ? pointA : pointB;

        // Flip to face direction
        if (transform.position.x > targetPoint.position.x)
            sprite.flipX = true;
        else
            sprite.flipX = false;

        // Move towards the point
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // Check if we reached the point
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            movingToA = !movingToA; // Switch target
        }
    }
}