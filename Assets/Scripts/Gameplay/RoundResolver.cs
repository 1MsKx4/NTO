using UnityEngine;

public class RoundResolver : MonoBehaviour
{
    public bool Compare(int card, int otherCard, Prediction prediction)
    {
        if (prediction == Prediction.Less)
        {
            return card < otherCard;
        }
        if (prediction == Prediction.Equal)
        {
            return card == otherCard;
        }
        return card > otherCard;
    }

    public Prediction GuestPrediction(int card)
    {
        if (card <= 5)
        {
            return Prediction.Less;
        }
        if (card >= 8)
        {
            return Prediction.Greater;
        }
        return (Prediction)Random.Range(0, 3);
    }
}

public enum Prediction
{
    Less,
    Equal,
    Greater
}

public enum RoundPhase
{
    Newspaper,
    Prediction,
    Shuffling,
    ThreeCards,
    Result,
    Ending
}
