using UnityEngine;

public class MatchScore : MonoBehaviour
{
    public GameSettings settings;
    public int Gold, Act, Guest, Rounds, GuestRound, GuestGold, Pledge;
    public bool Finished, GuestLeaving;
    public string Ending, PledgeMessage;

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
        Pledge = 0;
        Finished = false;
        Ending = "";
        StartGuest();
        return true;
    }

    public ActSettings GetAct()
    {
        return settings.acts[Act - 1];
    }

    public GuestSettings GetGuest()
    {
        return GetAct().guests[Guest - 1];
    }

    public void StartGuest()
    {
        GuestRound = 1;
        GuestGold = GetGuest().gold;
        GuestLeaving = false;
        PledgeMessage = "";
    }

    public int GetStake(int index)
    {
        return Mathf.Min(GetAct().stakes[index], Mathf.Min(Gold, GuestGold / 2));
    }

    public bool StakeAvailable(int index)
    {
        if (Gold < 15000 && index != GetAct().stakes.Length - 1)
        {
            return false;
        }
        if (Gold < 30000 && index == 0 && GetAct().stakes.Length > 1)
        {
            return false;
        }
        return GetStake(index) > 0;
    }

    public void AddRound(int change, int fine, bool caught)
    {
        Gold += change;
        GuestGold -= change;
        Gold = Mathf.Max(0, Gold - fine);
        Rounds++;
        PledgeMessage = "";
        GuestLeaving = caught || GuestRound >= GetGuest().rounds || GuestGold < 2;

        if (Gold == 0)
        {
            if (Act == 4)
            {
                Finished = true;
                Ending = "Моральное разложение. Тройка… семёрка… туз…";
            }
            else if (Pledge < settings.pledges.Length)
            {
                Gold = settings.pledges[Pledge];
                Pledge++;
                if (Pledge == 1)
                {
                    PledgeMessage = "Заложены украшения";
                }
                else if (Pledge == 2)
                {
                    PledgeMessage = "Заложена утварь";
                }
                else
                {
                    PledgeMessage = "Заложен дом. Последний шанс";
                }
            }
            else
            {
                Finished = true;
                Ending = "Потеря дома. Ключ забирают, дверь кабинета закрывается.";
            }
        }
        if (!Finished && Act == 4 && Guest == GetAct().guests.Length && GuestLeaving)
        {
            Finished = true;
            Ending = "Моральное разложение. Тройка… семёрка… туз…";
        }
    }

    public bool NextRound()
    {
        if (GuestLeaving)
        {
            Guest++;
            if (Guest > GetAct().guests.Length)
            {
                Act++;
                Guest = 1;
            }
            StartGuest();
            return true;
        }
        GuestRound++;
        return false;
    }
}
