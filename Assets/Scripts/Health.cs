using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Invincibility (For Player)")]
    public float invincibilityDuration = 1f; 
    private float nextDamageTime = 0f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this method to deal damage
    public void TakeDamage(float damage)
    {
       
        if (Time.time < nextDamageTime)
            return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took damage! Health is now: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            
            nextDamageTime = Time.time + invincibilityDuration;
        }
    }

    void Die()
    {
        // CHECK TAGS TO DECIDE WHAT HAPPENS
        if (gameObject.CompareTag("Player"))
        {
            
            PlayerMovement playerScript = GetComponent<PlayerMovement>();
            if (playerScript != null)
            {
                playerScript.Respawn();
                
                currentHealth = maxHealth; 
            }
        }
        else
        {
            
            Destroy(gameObject);
        }
    }
}