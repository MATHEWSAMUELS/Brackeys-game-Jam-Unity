using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 20f;
    public float lifeTime = 2f;
    public float damage = 25f; // Added damage amount

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ensure gravity is off so it flies straight
        rb.gravityScale = 0f; 
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        // Move the bullet locally to the right
        rb.linearVelocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if we hit an Enemy
        if (collision.CompareTag("Enemy"))
        {
            // --- CHANGED: Instead of destroying, we deal damage ---
            Health enemyHealth = collision.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            
            // Destroy the bullet after it hits
            Destroy(gameObject);
        }

        // 2. Check if we hit a wall/floor
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}