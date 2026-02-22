using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int totalPlayableLevels = 3;
    public static void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene("Level" + levelIndex);
    }

    public static void CompleteLevel(int currentLevel)
    {
        // Don’t unlock more than total playable levels
        int nextLevel = Mathf.Clamp(currentLevel + 1, 1, totalPlayableLevels);

        PlayerPrefs.SetInt("CurrentLevel", nextLevel);
    }

    public static int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt("CurrentLevel", 1); // default = 1
    }

    public static void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}