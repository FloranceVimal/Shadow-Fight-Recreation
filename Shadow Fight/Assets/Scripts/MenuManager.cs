using UnityEngine;
using UnityEngine.SceneManagement; // We need this to change scenes!

public class MenuManager : MonoBehaviour
{
    public void StartSinglePlayer()
    {
        // Write a sticky note saying "Yes, we want AI" (1 = True)
        PlayerPrefs.SetInt("IsSinglePlayer", 1);
        
        // Load the combat scene!
        SceneManager.LoadScene("FightScene");
    }

    public void StartMultiplayer()
    {
        // Write a sticky note saying "No, we want a human Player 2" (0 = False)
        PlayerPrefs.SetInt("IsSinglePlayer", 0);
        
        SceneManager.LoadScene("FightScene");
    }
}