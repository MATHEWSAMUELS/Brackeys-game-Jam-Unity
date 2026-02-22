using UnityEngine;

public class SecretPath : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] objectsToReveal; // Drag your Platform and Asset here
    public bool hideOnStart = true;      // Keeps them invisible until triggered

    void Start()
    {
        // 1. Hide them at the start of the game
        if (hideOnStart)
        {
            foreach (GameObject obj in objectsToReveal)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 2. Reveal them when player touches this trigger
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in objectsToReveal)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }

            // 3. Destroy this trigger
            Destroy(gameObject);
        }
    }
}