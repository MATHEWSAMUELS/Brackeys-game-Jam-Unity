using UnityEngine;
using UnityEngine.UI;

public class LevelUnlocker : MonoBehaviour
{
    public Button level2Button;
    public Button level3Button;

    void Start()
    {
        int unlocked = LevelManager.GetUnlockedLevel();

        level2Button.interactable = unlocked >= 2;
        level3Button.interactable = unlocked >= 3;
    }
}