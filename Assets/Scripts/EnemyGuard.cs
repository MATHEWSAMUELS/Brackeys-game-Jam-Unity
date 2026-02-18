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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

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
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (canShoot)
                StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 shootDirection = (player.position - firePoint.position).normalized;
        bullet.GetComponent<EnemyProjectile>().SetDirection(shootDirection);

        yield return new WaitForSeconds(shootCooldown);
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

        // Disable shooting and patrol
        StopAllCoroutines();
    }
}
