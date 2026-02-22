using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Replace("Level", ""));

            // If this is the final level → go directly to Victory
            if (currentLevel >= LevelManager.totalPlayableLevels)
            {
                SceneManager.LoadScene("Victory");
                return;
            }

            // Otherwise unlock next level
            LevelManager.CompleteLevel(currentLevel);

            // Load regular Win screen
            SceneManager.LoadScene("WinScreen");
        }
    }
}