using UnityEngine;
using UnityEngine.UI;

public class CardTableView : MonoBehaviour
{
    public Text playerCardText, opponentCardText, tableCardText, roundResultText, scoreText;
    public Text suspicionText, sleeveText, bankText;
    public Image suspicionFill;
    public GameObject suspicionBar;
    public Button lessButton, equalButton, greaterButton, nextRoundButton, skipButton;
    public Button teaButton, bluffButton;
    public Button[] stakes, tableCards;
    public Text[] stakeLabels, cardLabels;
    public Text nextLabel, teaLabel;
    public GameObject sleeve;
    public GameObject background, newspaperScreen;
    public RoundController game;
    public CheatController cheats;
    public Camera gameCamera;

    private bool playing, prediction, table, active;
    private int amount;
    private ColorBlock colors;
    private string[] signs = { "<", "=", ">" };

    public void Show()
    {
        playing = game.phase != RoundPhase.Newspaper;
        background.SetActive(playing);
        newspaperScreen.SetActive(!playing);
        roundResultText.gameObject.SetActive(playing);
        if (!playing)
        {
            gameCamera.backgroundColor = Color.black;
        }
        prediction = game.phase == RoundPhase.Prediction;
        table = game.phase == RoundPhase.ThreeCards || game.phase == RoundPhase.Shuffling ||
            game.phase == RoundPhase.Result && game.table.revealed;

        scoreText.gameObject.SetActive(playing);
        opponentCardText.gameObject.SetActive(playing);
        playerCardText.gameObject.SetActive(playing);
        tableCardText.gameObject.SetActive(playing);
        bankText.gameObject.SetActive(playing);
        suspicionText.gameObject.SetActive(playing);
        suspicionBar.SetActive(playing);
        roundResultText.text = game.message;
        nextRoundButton.gameObject.SetActive(playing && !prediction &&
            game.phase != RoundPhase.Shuffling && game.phase != RoundPhase.ThreeCards);

        nextLabel.text = "Следующий кон";
        if (game.phase == RoundPhase.Newspaper)
        {
            nextLabel.text = "Открыть газету";
        }
        else if (game.phase == RoundPhase.Ending)
        {
            nextLabel.text = "Начать заново";
        }
        else if (game.match.GuestLeaving)
        {
            nextLabel.text = "Следующий гость";
        }

        lessButton.gameObject.SetActive(prediction);
        equalButton.gameObject.SetActive(prediction);
        greaterButton.gameObject.SetActive(prediction);
        lessButton.interactable = game.stake > 0 && !cheats.holding;
        equalButton.interactable = lessButton.interactable;
        greaterButton.interactable = lessButton.interactable;
        for (int i = 0; i < stakes.Length; i++)
        {
            active = playing && i < game.match.GetAct().stakes.Length;
            stakes[i].gameObject.SetActive(active);
            if (active)
            {
                amount = game.match.GetStake(i);
                stakeLabels[i].text = amount.ToString("N0");
                if (game.match.Gold < 15000)
                {
                    stakeLabels[i].text = "Ва-банк " + amount.ToString("N0");
                }
                stakes[i].interactable = prediction && game.stake == 0 && game.match.StakeAvailable(i);
                colors = stakes[i].colors;
                colors.normalColor = Color.white;
                colors.disabledColor = new Color(0.55f, 0.55f, 0.55f);
                if (game.selectedStake == i)
                {
                    colors.disabledColor = new Color(0.9f, 0.74f, 0.45f);
                }
                stakes[i].colors = colors;
            }
        }

        teaButton.gameObject.SetActive(prediction || game.phase == RoundPhase.ThreeCards);
        teaButton.interactable = !cheats.teaUsed && game.stake > 0;
        bluffButton.gameObject.SetActive(prediction);
        bluffButton.interactable = !cheats.bluffUsed && game.stake > 0 && !cheats.holding;
        skipButton.gameObject.SetActive(prediction);
        skipButton.interactable = !cheats.skipUsed && game.match.Gold >= 30000 &&
            !cheats.roundCheated && !cheats.holding;
        sleeve.SetActive(prediction && game.match.Act >= 2);

        for (int i = 0; i < tableCards.Length; i++)
        {
            tableCards[i].gameObject.SetActive(table);
            tableCards[i].interactable = game.phase == RoundPhase.ThreeCards;
            cardLabels[i].text = "?";
            if ((game.table.showing || game.table.revealed || cheats.holding &&
                cheats.seenCards[i] && cheats.teaLeft > 0))
            {
                cardLabels[i].text = "Пусто";
                if (i == game.table.correctCard)
                {
                    cardLabels[i].text = game.guestCard.ToString();
                }
            }
        }

        if (playing)
        {
            playerCardText.text = $"<size=26>Ваша карта</size>\n<size=64>{game.playerCard}</size>";
            opponentCardText.text = game.match.GetGuest().guestName + "\nКарта: ?";
            if (cheats.guestVisible || game.predictionsMade)
            {
                opponentCardText.text = game.match.GetGuest().guestName + $"\nКарта: {game.guestCard}";
            }
            sleeveText.text = $"Рукав: {game.sleeveCard}\nПеретащить на свою карту";
            scoreText.text = $"Золото: {game.match.Gold:N0}    Акт {game.match.Act}    Гость {game.match.Guest}/{game.match.GetAct().guests.Length}    Кон {game.match.GuestRound}/{game.match.GetGuest().rounds}";
            bankText.text = $"Гость: {game.match.GuestGold:N0}    Банк: {game.stake * 2:N0}";
            if (game.phase == RoundPhase.Result || game.phase == RoundPhase.Ending)
            {
                bankText.text = $"У гостя осталось: {game.match.GuestGold:N0}";
            }
            tableCardText.text = "Моя карта относительно карты гостя";
            if (game.predictionsMade)
            {
                tableCardText.text = $"Вы: {signs[(int)game.playerPrediction]}    Гость: {signs[(int)game.guestPrediction]}";
            }
            gameCamera.backgroundColor = new Color(0.18f, 0.20f, 0.12f);
            if (game.match.Gold <= 40000)
            {
                gameCamera.backgroundColor = new Color(0.13f, 0.15f, 0.14f);
            }
            if (game.match.Act == 4)
            {
                gameCamera.backgroundColor = new Color(0.13f, 0.08f, 0.23f);
            }
            ShowRisk();
        }
    }

    public void ShowRisk()
    {
        suspicionText.text = $"Подозрение: {cheats.suspicion:0}%";
        teaLabel.text = "Чашка чая";
        if (cheats.teaLeft > 0)
        {
            teaLabel.text = $"Отвернулся: {cheats.teaLeft:0.0} с";
        }
        else if (cheats.teaUsed)
        {
            teaLabel.text = "Чашка потрачена";
        }
        suspicionFill.rectTransform.anchorMax = new Vector2(1, cheats.suspicion / 100f);
        suspicionFill.color = Color.Lerp(new Color(0.79f, 0.65f, 0.36f), Color.red, cheats.suspicion / 100f);
    }
}
