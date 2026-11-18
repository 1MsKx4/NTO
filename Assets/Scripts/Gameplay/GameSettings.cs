using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Card Game/Game Settings")]
public class GameSettings : ScriptableObject
{
    public int startingGold = 60000, jackpot = 100000;
    public int equalMultiplier = 4, edgeBonus = 25;
    public int normalRisk = 35, distractedRisk = 15, repeatTeaRisk = 30;
    public float teaSeconds = 5f;
    public ActSettings[] acts = new ActSettings[0];

    public bool CheckSettings()
    {
        if (startingGold <= 0 || jackpot <= 0)
        {
            return false;
        }
        if (equalMultiplier < 1 || equalMultiplier > 10 || edgeBonus < 0 || edgeBonus > 100)
        {
            return false;
        }
        if (normalRisk < 0 || normalRisk > 100 || distractedRisk < 0 ||
            distractedRisk > normalRisk || repeatTeaRisk < 0 || repeatTeaRisk > 100 ||
            !(teaSeconds > 0 && teaSeconds <= 60))
        {
            return false;
        }
        if (acts.Length != 4)
        {
            return false;
        }

        for (int i = 0; i < acts.Length; i++)
        {
            ActSettings act = acts[i];
            if (act.stakes.Length != 3)
            {
                return false;
            }
            if (act.stakes[0] <= 0 || act.stakes[0] >= act.stakes[1] ||
                act.stakes[1] >= act.stakes[2] || act.stakes[2] > 10000000)
            {
                return false;
            }
            if (act.fine < 0 || act.fine > 10000000 || act.guests.Length == 0)
            {
                return false;
            }
            foreach (GuestSettings guest in act.guests)
            {
                if (string.IsNullOrWhiteSpace(guest.guestName) ||
                    guest.extraSuspicion < 0 || guest.extraSuspicion > 100)
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
    public GuestSettings[] guests = new GuestSettings[0];
}

[System.Serializable]
public class GuestSettings
{
    public string guestName;
    public int extraSuspicion;
}
