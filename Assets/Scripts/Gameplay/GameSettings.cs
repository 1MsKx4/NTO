using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Card Game/Game Settings")]
public class GameSettings : ScriptableObject
{
    public int startingGold = 60000;
    public int[] pledges = { 10000, 10000, 20000 };
    public int equalMultiplier = 2, secondPhaseMultiplier = 2;
    public float peekSeconds = 0.4f, showCardSeconds = 0.5f;
    public float teaSuspicion = 20f, handSuspicion = 50f, honestRelief = 15f;
    public float opponentFindChance = 0.33f, middleBluffChance = 0.65f, edgeBluffChance = 0.1f;
    public ActSettings[] acts = new ActSettings[0];

    public bool CheckSettings()
    {
        if (startingGold <= 0 || startingGold > 10000000 || pledges.Length != 3)
        {
            return false;
        }
        foreach (int pledge in pledges)
        {
            if (pledge <= 0 || pledge > 10000000)
            {
                return false;
            }
        }
        if (equalMultiplier != 2 || secondPhaseMultiplier != 2 ||
            peekSeconds <= 0 || showCardSeconds <= 0 || acts.Length != 4 ||
            teaSuspicion < 0 || teaSuspicion > 100 || handSuspicion <= 0 ||
            honestRelief < 0 || honestRelief > 100 ||
            opponentFindChance < 0 || opponentFindChance > 1 ||
            middleBluffChance < 0 || middleBluffChance > 1 ||
            edgeBluffChance < 0 || edgeBluffChance > 1)
        {
            return false;
        }
        for (int i = 0; i < acts.Length; i++)
        {
            ActSettings act = acts[i];
            if (act.stakes.Length == 0 || act.stakes.Length > 3 || act.guests.Length == 0 ||
                act.fine < 0 || act.fine > 10000000 || act.teaSeconds <= 0 ||
                act.shuffleSeconds <= 0 || act.shuffleMoves < 1 || act.shuffleMoves > 10)
            {
                return false;
            }
            for (int j = 0; j < act.stakes.Length; j++)
            {
                if (act.stakes[j] <= 0 || act.stakes[j] > 10000000)
                {
                    return false;
                }
                if (j > 0 && act.stakes[j] <= act.stakes[j - 1])
                {
                    return false;
                }
            }
            foreach (GuestSettings guest in act.guests)
            {
                if (string.IsNullOrWhiteSpace(guest.guestName) || guest.rounds < 1 || guest.rounds > 3 ||
                    guest.extraSuspicion < 0 || guest.extraSuspicion > 100 ||
                    guest.gold < 2 * act.stakes[act.stakes.Length - 1] || guest.gold > 10000000)
                {
                    return false;
                }
            }
        }
        return true;
    }
}

[System.Serializable]
public class ActSettings
{
    public int[] stakes = new int[0];
    public int fine;
    public float teaSeconds, shuffleSeconds;
    public int shuffleMoves;
    public GuestSettings[] guests = new GuestSettings[0];
}

[System.Serializable]
public class GuestSettings
{
    public string guestName;
    public int rounds = 3, gold = 60000, extraSuspicion;
}
