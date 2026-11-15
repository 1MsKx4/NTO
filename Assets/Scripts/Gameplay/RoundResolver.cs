using UnityEngine;

public class RoundResolver : MonoBehaviour
{
    private bool playerCorrect, opponentCorrect;

    public RoundOutcome Resolve(
        int playerCard, Prediction playerPrediction,
        int opponentCard, Prediction opponentPrediction,
        int tableCard)
    {
        playerCorrect = Compare(playerCard, tableCard) == playerPrediction;
        opponentCorrect = Compare(opponentCard, tableCard) == opponentPrediction;

        if (playerCorrect == opponentCorrect)
        {
            return RoundOutcome.Draw;
        }

        if (playerCorrect)
        {
            return RoundOutcome.PlayerWins;
        }
        return RoundOutcome.OpponentWins;
    }

    private Prediction Compare(int handCard, int tableCard)
    {
        if (handCard < tableCard)
        {
            return Prediction.Less;
        }
        if (handCard == tableCard)
        {
            return Prediction.Equal;
        }

        return Prediction.Greater;
    }
}

public enum Prediction
{
    Less,
    Equal,
    Greater
}

public enum RoundOutcome
{
    Draw,
    PlayerWins,
    OpponentWins
}
