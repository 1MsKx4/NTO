using UnityEngine;

public class MatchScore : MonoBehaviour
{
    public GameSettings settings;

    public int Gold, Act, Guest, Rounds;
    public bool Finished;
    public string Ending;

    public ActSettings CurrentAct => settings.acts[Act - 1];
    public GuestSettings CurrentGuest => CurrentAct.guests[Guest - 1];

    public bool ResetMatch()
    {
        if (!settings.CheckSettings())
        {
            return false;
        }

        Gold = settings.startingGold;
        Act = 1;
        Guest = 1;
        Rounds = 0;
        Finished = false;
        Ending = "";
        return true;
    }

    public int GetStake(int index)
    {
        return Mathf.Min(CurrentAct.stakes[index], Gold);
    }

    public bool LowGold()
    {
        return 2L * Gold < settings.startingGold;
    }

    public void AddRound(int change)
    {
        Gold = (int)System.Math.Max(0, System.Math.Min(int.MaxValue, (long)Gold + change));
        Rounds++;

        if (Gold == 0 && Act < 4)
        {
            Finished = true;
            Ending = "Потеря дома. Украшения, утварь и ключ уходят за долги.";
        }
        else if (Act == 4 && Guest == CurrentAct.guests.Length)
        {
            Finished = true;
            Ending = "Моральное разложение. Тройка… семёрка… туз…";
        }
    }

    public void NextGuest()
    {
        if (Guest < CurrentAct.guests.Length)
        {
            Guest++;
        }
        else if (Act < 3 || (Act == 3 && Gold >= settings.jackpot))
        {
            Act++;
            Guest = 1;
        }
    }
}
