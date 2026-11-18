using UnityEngine;

public class RoundResolver : MonoBehaviour
{
    public bool Resolve(int openCard, int nextCard, Prediction prediction)
    {
        if (prediction == Prediction.Less)
        {
            return nextCard < openCard;
        }
        if (prediction == Prediction.Equal)
        {
            return nextCard == openCard;
        }

        return nextCard > openCard;
    }
}

public enum Prediction
{
    Less,
    Equal,
    Greater
}
