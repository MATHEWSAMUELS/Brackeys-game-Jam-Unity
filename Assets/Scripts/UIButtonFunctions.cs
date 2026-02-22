using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonFunctions : MonoBehaviour
{
    public void PlayGame()
    {
        int level = Mathf.Clamp(LevelManager.GetUnlockedLevel(), 1, LevelManager.totalPlayableLevels);
        SceneManager.LoadScene("Level" + level);
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

        // If in WinScreen, replay the previous level
        if (SceneManager.GetActiveScene().name == "WinScreen")
            level--;

        // If in Victory, replay final level
        if (SceneManager.GetActiveScene().name == "Victory")
            level = LevelManager.totalPlayableLevels;

        SceneManager.LoadScene("Level" + level);
    }

    public void Next()
    {
        int nextLevel = LevelManager.GetUnlockedLevel();

        // If the next level is BEYOND final playable level → go to Victory screen
        if (nextLevel > LevelManager.totalPlayableLevels)
        {
            SceneManager.LoadScene("Victory");
            return;
        }

        // Otherwise load the next level normally
        LevelManager.LoadLevel(nextLevel);
    }

    public void BackToMenu()
    {
        LevelManager.LoadMenu();
    }
}