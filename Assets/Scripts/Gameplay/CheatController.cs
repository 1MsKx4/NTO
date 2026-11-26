using UnityEngine;

public class CheatController : MonoBehaviour
{
    public GameSettings settings;
    public float suspicion, teaLeft;
    public bool teaUsed, bluffUsed, skipUsed, holding, roundCheated, guestVisible;
    public bool changed, caught, swapCard;
    public string message;
    public bool[] seenCards = new bool[3];
    public int cardsSeen;
    private int target;
    private float heldSeconds;
    private bool performed;

    public void ClearResult()
    {
        changed = false;
        caught = false;
        swapCard = false;
        message = "";
    }

    public void UseBluff()
    {
        bluffUsed = true;
        roundCheated = true;
    }

    public void UseSkip()
    {
        skipUsed = true;
    }

    public void ResetGuest()
    {
        suspicion = 0;
        teaUsed = false;
        bluffUsed = false;
        skipUsed = false;
    }

    public void ResetRound()
    {
        teaLeft = 0;
        holding = false;
        roundCheated = false;
        guestVisible = false;
        changed = false;
        caught = false;
        swapCard = false;
        message = "";
        cardsSeen = 0;
        target = 0;
        heldSeconds = 0;
        performed = false;
        for (int i = 0; i < seenCards.Length; i++)
        {
            seenCards[i] = false;
        }
    }

    public void Tea(float seconds)
    {
        teaUsed = true;
        roundCheated = true;
        teaLeft = seconds;
        suspicion = Mathf.Min(100, suspicion + settings.teaSuspicion);
        changed = true;
        if (suspicion >= 100)
        {
            Catch();
        }
        else
        {
            message = "Гость отвернулся. Удерживайте карту, затем отпустите до конца таймера.";
        }
    }

    public void Hold(int card)
    {
        if (card >= 0 && !seenCards[card] && cardsSeen >= 2)
        {
            message = "За чашку можно подсмотреть только две карты.";
            changed = true;
        }
        else
        {
            target = card;
            holding = true;
            performed = false;
            heldSeconds = 0;
            roundCheated = true;
            changed = true;
        }
    }

    public void Release(bool dropped, bool sleeveAvailable)
    {
        if (holding)
        {
            if (target == -2)
            {
                if (dropped && teaLeft > 0 && sleeveAvailable)
                {
                    swapCard = true;
                }
                else if (teaLeft <= 0)
                {
                    message = "Гость уже смотрит. Подмена сорвалась.";
                }
                else
                {
                    message = "Карта вернулась в рукав. Наложите её на вашу карту.";
                }
            }
            CancelHold();
            changed = true;
        }
    }

    public void Tick(int extraSuspicion)
    {
        if (teaLeft > 0)
        {
            teaLeft = Mathf.Max(0, teaLeft - Time.deltaTime);
            if (teaLeft == 0)
            {
                message = "Гость снова смотрит.";
                changed = true;
            }
        }
        if (holding)
        {
            if (teaLeft <= 0)
            {
                suspicion = Mathf.Min(100, suspicion +
                    (settings.handSuspicion + extraSuspicion) * Time.deltaTime);
                guestVisible = false;
                if (suspicion >= 100)
                {
                    Catch();
                }
            }
            else
            {
                heldSeconds += Time.deltaTime;
                if (!performed && heldSeconds >= settings.peekSeconds)
                {
                    performed = true;
                    if (target == -1)
                    {
                        guestVisible = true;
                    }
                    else if (target >= 0 && !seenCards[target])
                    {
                        seenCards[target] = true;
                        cardsSeen++;
                    }
                }
            }
            changed = true;
        }
    }

    private void Catch()
    {
        CancelHold();
        caught = true;
        changed = true;
        message = "Поимка. Ставка сгорела, штраф, гость уходит.";
    }

    public void CancelHold()
    {
        holding = false;
        guestVisible = false;
    }

    public void EndRound()
    {
        CancelHold();
        teaLeft = 0;
        if (!roundCheated)
        {
            suspicion = Mathf.Max(0, suspicion - settings.honestRelief);
        }
    }
}
