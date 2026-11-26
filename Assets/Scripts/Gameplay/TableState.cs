public class TableState
{
    public RoundPhase phase;
    public Prediction playerPrediction, guestPrediction;
    public int playerCard, guestCard, sleeveCard, stake, selectedStake;
    public int gold, guestGold, act, guest, guestCount, guestRound, guestRounds;
    public int stakeCount, correctCard;
    public string message, guestName;
    public bool predictionsMade, guestLeaving, guestVisible, holding, teaUsed;
    public bool showing, revealed, allIn, canGuess, canTea, canBluff, canSkip, showSleeve;
    public float suspicion, teaLeft;
    public int[] stakes = new int[3];
    public bool[] stakeAvailable = new bool[3], seenCards = new bool[3];
}
