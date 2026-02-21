using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene("Level" + levelIndex);
    }

    public static void CompleteLevel(int currentLevel)
    {
        // unlock next level
        int nextLevel = currentLevel + 1;
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