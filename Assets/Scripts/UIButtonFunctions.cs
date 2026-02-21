using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonFunctions : MonoBehaviour
{
    public void PlayGame()
    {
        int level = LevelManager.GetUnlockedLevel();
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
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void Next()
    {
        int level = int.Parse(SceneManager.GetActiveScene().name.Replace("Level", ""));
        LevelManager.CompleteLevel(level);
        LevelManager.LoadLevel(level + 1);
    }

    public void BackToMenu()
    {
        LevelManager.LoadMenu();
    }
}