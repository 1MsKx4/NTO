using UnityEngine;

public class RoundController : MonoBehaviour
{
    public MatchScore match;
    public RoundResolver resolver;
    public CheatController cheats;
    public ThreeCardGame table;
    public CardTableView view;
    public RoundPhase phase;
    public int playerCard, guestCard, sleeveCard, stake, selectedStake;
    public Prediction playerPrediction, guestPrediction;
    public bool predictionsMade;
    public string message;
    private CardDeck playerDeck = new CardDeck(), guestDeck = new CardDeck();
    private bool playerCorrect, guestCorrect, newGuest;
    private int change, savedCard;
    private int[] sleeveCards = { 1, 2, 11, 12 };
    private float bluffChance;

    private void Start()
    {
        Newspaper();
    }

    private void Update()
    {
        if (phase == RoundPhase.Prediction || phase == RoundPhase.ThreeCards)
        {
            cheats.Tick(match.GetGuest().extraSuspicion);
            RefreshCheats();
        }
        if (phase == RoundPhase.Shuffling && table.changed)
        {
            table.changed = false;
            if (table.ready)
            {
                phase = RoundPhase.ThreeCards;
                message = "Найдите карту гостя среди трёх. Для подсмотра сначала используйте чашку.";
            }
            view.Show();
        }
    }

    public void Tea()
    {
        if (!cheats.teaUsed && stake > 0 &&
            (phase == RoundPhase.Prediction || phase == RoundPhase.ThreeCards))
        {
            cheats.Tea(match.GetAct().teaSeconds);
            RefreshCheats();
        }
    }

    public void HoldCard(int card)
    {
        if (!cheats.holding && stake > 0 &&
            (phase == RoundPhase.Prediction && card < 0 ||
             phase == RoundPhase.ThreeCards && card >= 0 && match.Act >= 2))
        {
            cheats.Hold(card);
            RefreshCheats();
        }
    }

    public void ReleaseCard(bool dropped)
    {
        cheats.Release(dropped, match.Act >= 2);
        RefreshCheats();
    }

    public void CancelHold()
    {
        cheats.CancelHold();
    }

    private void RefreshCheats()
    {
        if (cheats.changed)
        {
            cheats.changed = false;
            if (cheats.message != "")
            {
                message = cheats.message;
                cheats.message = "";
            }
            if (cheats.swapCard)
            {
                cheats.swapCard = false;
                savedCard = playerCard;
                playerCard = sleeveCard;
                sleeveCard = savedCard;
                message = $"Подмена: ваша карта теперь {playerCard}, в рукаве {sleeveCard}.";
            }
            if (cheats.caught)
            {
                cheats.caught = false;
                Finish(-stake, match.GetAct().fine, true);
            }
            else
            {
                view.Show();
            }
        }
        else if (cheats.teaUsed)
        {
            view.ShowRisk();
        }
    }

    public void Newspaper()
    {
        phase = RoundPhase.Newspaper;
        message = "Газета: «Граф желает играть»";
        view.Show();
    }

    public void Continue()
    {
        if (phase == RoundPhase.Ending)
        {
            Newspaper();
        }
        else if (phase == RoundPhase.Newspaper)
        {
            if (match.ResetMatch())
            {
                playerDeck.Shuffle();
                guestDeck.Shuffle();
                NewGuest();
                Deal();
            }
        }
        else if (phase == RoundPhase.Result)
        {
            newGuest = match.NextRound();
            if (newGuest)
            {
                NewGuest();
            }
            Deal();
        }
    }

    private void NewGuest()
    {
        cheats.ResetGuest();
        sleeveCard = sleeveCards[Random.Range(0, sleeveCards.Length)];
    }

    private void Deal()
    {
        playerCard = playerDeck.Draw();
        guestCard = guestDeck.Draw();
        stake = 0;
        selectedStake = -1;
        predictionsMade = false;
        table.showing = false;
        table.revealed = false;
        cheats.ResetRound();
        phase = RoundPhase.Prediction;
        message = "Выберите ставку. Прогноз относится к вашей карте относительно карты гостя.";
        view.Show();
    }

    public void SelectStake(int index)
    {
        if (phase == RoundPhase.Prediction && stake == 0 && match.StakeAvailable(index))
        {
            selectedStake = index;
            stake = match.GetStake(index);
            message = $"Вы поставили {stake:N0}, гость поставил столько же. Сделайте прогноз.";
            view.Show();
        }
    }

    public void Guess(Prediction prediction)
    {
        if (phase == RoundPhase.Prediction && stake > 0 && !cheats.holding)
        {
            playerPrediction = prediction;
            predictionsMade = true;
            guestPrediction = resolver.GuestPrediction(guestCard);
            playerCorrect = resolver.Compare(playerCard, guestCard, prediction);
            guestCorrect = resolver.Compare(guestCard, playerCard, guestPrediction);
            if (playerCorrect && guestCorrect)
            {
                phase = RoundPhase.Shuffling;
                message = "Оба угадали. Запомните карту гостя и следите за тасовкой.";
                view.Show();
                table.Begin(match.settings.showCardSeconds,
                    match.GetAct().shuffleSeconds, match.GetAct().shuffleMoves);
            }
            else
            {
                change = 0;
                message = "Оба ошиблись. Ничья";
                if (playerCorrect)
                {
                    change = stake;
                    message = "Угадали только вы";
                    if (prediction == Prediction.Equal)
                    {
                        change *= match.settings.equalMultiplier;
                    }
                }
                else if (guestCorrect)
                {
                    change = -stake;
                    message = "Угадал только гость";
                }
                Finish(change, 0, false);
            }
        }
    }

    public void GuessLess()
    {
        Guess(Prediction.Less);
    }

    public void GuessEqual()
    {
        Guess(Prediction.Equal);
    }

    public void GuessGreater()
    {
        Guess(Prediction.Greater);
    }

    public void Bluff()
    {
        if (phase == RoundPhase.Prediction && stake > 0 && !cheats.bluffUsed && !cheats.holding)
        {
            cheats.bluffUsed = true;
            cheats.roundCheated = true;
            bluffChance = match.settings.edgeBluffChance;
            if (guestCard >= 4 && guestCard <= 9)
            {
                bluffChance = match.settings.middleBluffChance;
            }
            if (Random.value < bluffChance)
            {
                message = "Гость поверил блефу и спасовал";
                Finish(stake / 2, 0, false);
            }
            else
            {
                stake = Mathf.Min(stake * 2, Mathf.Min(match.Gold, match.GuestGold / 2));
                message = $"Гость не поверил. Ставка выросла до {stake:N0}. Выберите прогноз.";
                view.Show();
            }
        }
    }

    public void Skip()
    {
        if (phase == RoundPhase.Prediction && !cheats.skipUsed && match.Gold >= 30000 &&
            !cheats.roundCheated && !cheats.holding)
        {
            cheats.skipUsed = true;
            message = "Кон пропущен";
            Finish(0, 0, false);
        }
    }

    public void ChooseCard(int index)
    {
        if (phase == RoundPhase.ThreeCards && !cheats.holding)
        {
            table.chosenCard = index;
            table.revealed = true;
            playerCorrect = index == table.correctCard;
            guestCorrect = Random.value < match.settings.opponentFindChance;
            change = 0;
            message = "Оба промахнулись. Ничья";
            if (playerCorrect && guestCorrect)
            {
                message = "Оба нашли карту. Ничья";
            }
            else if (playerCorrect)
            {
                change = stake * match.settings.secondPhaseMultiplier;
                message = "Только вы нашли карту";
            }
            else if (guestCorrect)
            {
                change = -Mathf.Min(match.Gold, stake * match.settings.secondPhaseMultiplier);
                message = "Только гость нашёл карту";
            }
            Finish(change, 0, false);
        }
    }

    public void Finish(int goldChange, int fine, bool caught)
    {
        if (phase == RoundPhase.Prediction || phase == RoundPhase.ThreeCards)
        {
            cheats.EndRound();
            match.AddRound(goldChange, fine, caught);
            message += $" · Золото: {goldChange - fine:+#;-#;0}";
            if (match.PledgeMessage != "")
            {
                message += "\n" + match.PledgeMessage;
            }
            phase = RoundPhase.Result;
            if (match.Finished)
            {
                phase = RoundPhase.Ending;
                message = match.Ending;
            }
            view.Show();
        }
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void RecordingAct(int act)
    {
        if (match.ResetMatch())
        {
            table.StopAllCoroutines();
            match.Act = act;
            match.StartGuest();
            playerDeck.Shuffle();
            guestDeck.Shuffle();
            NewGuest();
            Deal();
        }
    }

    public void RecordingGold(int gold)
    {
        if (phase == RoundPhase.Newspaper || phase == RoundPhase.Ending)
        {
            RecordingAct(1);
        }
        if (phase != RoundPhase.Newspaper && phase != RoundPhase.Ending)
        {
            table.StopAllCoroutines();
            match.Gold = gold;
            Deal();
            if (gold == 0)
            {
                message = "Золото закончилось";
                Finish(0, 0, false);
            }
        }
    }
#endif
}
