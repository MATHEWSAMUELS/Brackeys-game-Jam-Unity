using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonFunctions : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource; // Drag the AudioSource here
    public AudioClip winSound;       // Drag Win Sound here
    public AudioClip loseSound;      // Drag Lose Sound here

    void Start()
    {
        // Detect which screen we are on and play the appropriate sound
        string currentScene = SceneManager.GetActiveScene().name;

        // If we are in a Win or Victory scene
        if (currentScene == "WinScreen" || currentScene == "Victory")
        {
            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }
        }
        // If we are in the Lose/Defeat scene (Matches your previous code)
        else if (currentScene == "LoseScene" || currentScene == "Defeat")
        {
            if (audioSource != null && loseSound != null)
            {
                audioSource.PlayOneShot(loseSound);
            }
        }
    }

    public void PlayGame()
    {
        // Always start at Level 1
        SceneManager.LoadScene("Level1"); 
    }

    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        LevelManager.QuitGame();
    }

    public void LoadLevel(int level)
    {
        LevelManager.LoadLevel(level);
    }

    public void Replay()
    {
        int level = LevelManager.GetUnlockedLevel();

        if (SceneManager.GetActiveScene().name == "WinScreen")
            level--;

        if (SceneManager.GetActiveScene().name == "Victory")
            level = LevelManager.totalPlayableLevels;

        SceneManager.LoadScene("Level" + level);
    }

    public void Next()
    {
        int nextLevel = LevelManager.GetUnlockedLevel();

        if (nextLevel > LevelManager.totalPlayableLevels)
        {
            SceneManager.LoadScene("Victory");
            return;
        }

        LevelManager.LoadLevel(nextLevel);
    }

    public void BackToMenu()
    {
        LevelManager.LoadMenu();
    }

    public void RestartFromDefeat()
    {
        string levelName = PlayerPrefs.GetString("PreviousLevel");

        if (string.IsNullOrEmpty(levelName))
        {
            levelName = "Level 1";
        }

        SceneManager.LoadScene(levelName);
    }
}