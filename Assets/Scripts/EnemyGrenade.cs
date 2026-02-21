using UnityEngine;
using Ilumisoft.HealthSystem;

public class EnemyGrenade : MonoBehaviour
{
    public float speed = 5f;
    public float arcHeight = 6f; 
    public int damage = 10;

    private Rigidbody2D rb;

    public void Launch(Vector2 direction)
    {
        if (rb == null) 
        {
            rb = GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Rigidbody2D missing from Grenade Prefab!");
                return;
            }
        }

        Vector2 velocity = direction.normalized * speed;
        velocity.y = arcHeight;

        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the NEW Ilumisoft Health component
            Ilumisoft.HealthSystem.Health playerHealth = 
                collision.gameObject.GetComponent<Ilumisoft.HealthSystem.Health>();

            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(damage);
            }
        }

        // Destroy after any collision
        Destroy(gameObject);
    }
}