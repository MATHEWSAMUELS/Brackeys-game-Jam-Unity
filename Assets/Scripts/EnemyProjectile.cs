using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 8f;
    public float lifetime = 3f;
    public float damage = 10f; // Added damage amount

    private Vector2 moveDirection;

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // --- CHANGED: Look for Health component instead of PlayerMovement ---
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
                // Deal damage to the player
                playerHealth.TakeDamage(damage);
            }

            // Destroy the bullet after hitting
            Destroy(gameObject);
        }
    }
}