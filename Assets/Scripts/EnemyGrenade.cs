using UnityEngine;

public class EnemyGrenade : MonoBehaviour
{
    public float speed = 5f;
    public float arcHeight = 6f; // How high the object throws
    public int damage = 10;

    private Rigidbody2D rb;

    public void Launch(Vector2 direction)
    {
        // We get the Rigidbody here, right when we launch.
        // We also check if it's null just in case.
        if (rb == null) 
        {
            rb = GetComponent<Rigidbody2D>();
            
            // Safety Check: If still null, something is wrong with the Prefab setup
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D missing from Grenade Prefab!");
                return;
            }
        }

        // Calculate the velocity:
        // 1. Move horizontally towards the player
        // 2. Move vertically UP to create the arc
        Vector2 velocity = direction.normalized * speed;
        velocity.y = arcHeight;

        // Apply the velocity to the Rigidbody
        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If it hits the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal Damage
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Destroy the grenade when it hits anything (ground or player)
        Destroy(gameObject);
    }
}