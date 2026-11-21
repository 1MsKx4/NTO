using UnityEngine;

public class CheatController : MonoBehaviour
{
    public RoundController game;
    public float suspicion, teaLeft;
    public bool teaUsed, bluffUsed, skipUsed, holding, roundCheated, guestVisible;
    public bool[] seenCards = new bool[3];
    public int cardsSeen;
    private int target, savedCard;
    private float heldSeconds;
    private bool performed;

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
        cardsSeen = 0;
        target = 0;
        heldSeconds = 0;
        performed = false;
        for (int i = 0; i < seenCards.Length; i++)
        {
            seenCards[i] = false;
        }
    }

    public void Tea()
    {
        if (!teaUsed && game.stake > 0 &&
            (game.phase == RoundPhase.Prediction || game.phase == RoundPhase.ThreeCards))
        {
            teaUsed = true;
            roundCheated = true;
            teaLeft = game.match.GetAct().teaSeconds;
            suspicion = Mathf.Min(100, suspicion + game.match.settings.teaSuspicion);
            if (suspicion >= 100)
            {
                Catch();
            }
            else
            {
                game.message = "Гость отвернулся. Удерживайте карту, затем отпустите до конца таймера.";
                game.view.Show();
            }
        }
    }

    public void Hold(int card)
    {
        if (!holding && game.stake > 0 &&
            (game.phase == RoundPhase.Prediction && card < 0 ||
             game.phase == RoundPhase.ThreeCards && card >= 0 && game.match.Act >= 2))
        {
            if (card >= 0 && !seenCards[card] && cardsSeen >= 2)
            {
                game.message = "За чашку можно подсмотреть только две карты.";
                game.view.Show();
            }
            else
            {
                target = card;
                holding = true;
                performed = false;
                heldSeconds = 0;
                roundCheated = true;
            }
        }
    }

    public void Release(bool dropped)
    {
        if (holding)
        {
            if (target == -2)
            {
                if (dropped && teaLeft > 0 && game.match.Act >= 2)
                {
                    savedCard = game.playerCard;
                    game.playerCard = game.sleeveCard;
                    game.sleeveCard = savedCard;
                    game.message = $"Подмена: ваша карта теперь {game.playerCard}, в рукаве {game.sleeveCard}.";
                }
                else if (teaLeft <= 0)
                {
                    game.message = "Гость уже смотрит. Подмена сорвалась.";
                }
                else
                {
                    game.message = "Карта вернулась в рукав. Наложите её на вашу карту.";
                }
            }
            holding = false;
            guestVisible = false;
            game.view.Show();
        }
    }

    private void Update()
    {
        if (teaLeft > 0)
        {
            teaLeft = Mathf.Max(0, teaLeft - Time.deltaTime);
        }
        if (holding)
        {
            if (teaLeft <= 0)
            {
                suspicion = Mathf.Min(100, suspicion +
                    (game.match.settings.handSuspicion + game.match.GetGuest().extraSuspicion) * Time.deltaTime);
                guestVisible = false;
                if (suspicion >= 100)
                {
                    Catch();
                }
            }
            else
            {
                heldSeconds += Time.deltaTime;
                if (!performed && heldSeconds >= game.match.settings.peekSeconds)
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
            game.view.Show();
        }
        else if (teaLeft > 0)
        {
            game.view.ShowRisk();
        }
        else if (teaUsed)
        {
            game.view.ShowRisk();
        }
    }

    private void Catch()
    {
        holding = false;
        guestVisible = false;
        game.message = "Поимка. Ставка сгорела, штраф, гость уходит.";
        game.Finish(-game.stake, game.match.GetAct().fine, true);
    }

    public void EndRound()
    {
        holding = false;
        guestVisible = false;
        teaLeft = 0;
        if (!roundCheated)
        {
            suspicion = Mathf.Max(0, suspicion - game.match.settings.honestRelief);
        }
    }
}
