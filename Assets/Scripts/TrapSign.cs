using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class TrapSign : MonoBehaviour
{
    [Header("The Sign")]
    public TextMeshPro worldText;      // Drag your 3D Text object here
    public string newMessage = "Now it's down!"; // What it changes to

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if Player entered the trigger zone
        if (other.CompareTag("Player"))
        {
            // Change the text on the sign
            worldText.text = newMessage;

            // Destroy this trigger so it doesn't happen again
            Destroy(gameObject);
        }
    }
}