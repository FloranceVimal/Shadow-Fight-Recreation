using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float matchTime = 99f;
    public TextMeshProUGUI timerText;
    
    [Header("Win/Loss UI")]
    public TextMeshProUGUI centerText; // The giant text in the middle
    
    [Header("Players")]
    public Health player1Health;
    public Health player2Health;

    private bool matchActive = true;

    void Start()
    {
        // Hide the giant text when the round starts!
        centerText.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (matchActive && matchTime > 0)
        {
            matchTime -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(matchTime).ToString();

            if (matchTime <= 0)
            {
                matchTime = 0;
                TimeUp();
            }
        }
    }

    private void TimeUp()
    {
        matchActive = false; // Stop the match
        
        // Who has more health?
        if (player1Health.currentHealth > player2Health.currentHealth)
        {
            EndMatch("TIME UP!\nPLAYER 1 WINS!");
        }
        else if (player2Health.currentHealth > player1Health.currentHealth)
        {
            EndMatch("TIME UP!\nPLAYER 2 WINS!");
        }
        else
        {
            EndMatch("TIME UP!\nDRAW!");
        }
    }

    // Our Health script will call this exact function when someone's HP hits 0
    public void Knockout(GameObject loser)
    {
        if (!matchActive) return; // Prevent double K.O. glitches
        
        matchActive = false; // Stop the clock!

        if (loser.CompareTag("Player2"))
        {
            EndMatch("K.O!\nPLAYER 1 WINS!");
        }
        else
        {
            EndMatch("K.O!\nPLAYER 2 WINS!");
        }
    }

    private void EndMatch(string message)
    {
        // 1. Show the giant text
        centerText.text = message;
        centerText.gameObject.SetActive(true);
        
        // 2. Shut off both players' brains so they stop fighting
        player1Health.GetComponent<Movement>().enabled = false;
        player2Health.GetComponent<Movement>().enabled = false;
    }
}