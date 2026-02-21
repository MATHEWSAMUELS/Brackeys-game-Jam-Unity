using UnityEngine;
using Ilumisoft.HealthSystem;   // IMPORTANT: use the new health system

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 8f;
    public float lifetime = 3f;
    public float damage = 10f;

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
            // NEW HEALTH SYSTEM
            Ilumisoft.HealthSystem.Health playerHealth =
                collision.GetComponent<Ilumisoft.HealthSystem.Health>();

            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}