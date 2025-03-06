using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static int playerScore = 0;

    public static void AddScore(int shotsTaken)
    {
        if (shotsTaken <= 5)
        {
            playerScore += 200;
        }
        else if (shotsTaken <= 10)
        {
            playerScore += 100;
        }
        else
        {
            playerScore += 50;
        }

        // Update the saved score immediately
        PlayerPrefs.SetInt("PlayerScore", playerScore);
        PlayerPrefs.Save();
    }

    public static void ResetScore()
    {
        playerScore = 0;
        PlayerPrefs.SetInt("PlayerScore", playerScore);
        PlayerPrefs.Save();
    }

    public static void LoadScore()
    {
        playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
    }
}
