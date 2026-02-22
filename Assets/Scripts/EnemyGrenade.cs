using UnityEngine;
using Ilumisoft.HealthSystem;

public class EnemyGrenade : MonoBehaviour
{
    public AudioClip explosionSound;
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
         if (explosionSound != null)
        {
          GameObject soundObj = new GameObject("ExplosionSound");
        
        // Add an Audio Source component to it
        AudioSource source = soundObj.AddComponent<AudioSource>();
        
        // Configure the Source
        source.clip = explosionSound;
        source.spatialBlend = 0.0f; // <--- CRITICAL: 0.0f forces it to be 2D (No volume drop-off)
        source.volume = 1.0f;       // <--- Set to normal volume
        
        // Play the sound
        source.Play();
        
        // Destroy the object after the sound finishes so it doesn't clog memory
        Destroy(soundObj, explosionSound.length + 0.1f);
        }
        
        Destroy(gameObject);
        // Destroy after any collision
        Destroy(gameObject);
    }
}