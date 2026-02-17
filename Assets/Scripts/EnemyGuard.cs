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
    public LayerMask playerLayer;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;

    private Transform player;
    private bool movingToB = true;
    private bool isAlerted = false;
    private bool canShoot = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Detect Player
        if (distanceToPlayer <= detectionRange)
        {
            isAlerted = true;
        }
        else
        {
            isAlerted = false;
        }

        if (isAlerted)
        {
            HandleAlert(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Transform target = movingToB ? pointB : pointA;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            patrolSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            movingToB = !movingToB;
        }
    }

    void HandleAlert(float distanceToPlayer)
    {
        if (distanceToPlayer > stopDistance)
        {
            // Move closer
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                patrolSpeed * Time.deltaTime
            );
        }
        else
        {
            // Stop and shoot
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

}
