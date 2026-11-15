using UnityEngine;
using UnityEngine.UI;

public class MatchScore : MonoBehaviour
{
    public Text scoreText;
    private int playerWins, opponentWins, rounds;

    private void Start()
    {
        scoreText.text = "Вы: 0   Противник: 0   Раунды: 0";
    }

    public void AddRound(RoundOutcome outcome)
    {
        rounds++;
        if (outcome == RoundOutcome.PlayerWins)
        {
            playerWins++;
        }
        else if (outcome == RoundOutcome.OpponentWins)
        {
            opponentWins++;
        }

        scoreText.text = $"Вы: {playerWins}   Противник: {opponentWins}   Раунды: {rounds}";
    }
}
