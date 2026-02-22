using UnityEngine;

public class Grenade : MonoBehaviour
{

    public AudioClip explosionSound;

    [Header("Grenade Stats")]
    public float fuseTime = 3f;       
    public float blastRadius = 5f;    
    public float explosionForce = 10f; 
    
    [Header("Damage Settings")]
    public float explosionDamage = 50f;
     

    void Update()
    {
        
        fuseTime -= Time.deltaTime;

       
        if (fuseTime <= 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (Collider2D obj in objectsInRange)
        {
            
            if (obj.CompareTag("Enemy"))
            {
                
                Health enemyHealth = obj.GetComponent<Health>();

                
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(explosionDamage); 
                }
            }
            
            
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = obj.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);
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
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}