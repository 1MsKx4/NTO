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
    public GameObject[] panels;
    public BackgroundEffect effect;
    public GameMusic music;
    public OpponentView opponent;
    public Camera gameCamera;

    private TableState data;
    private bool playing, prediction, table, active;
    private int amount;
    private ColorBlock colors;
    private string[] signs = { "<", "=", ">" };

    public void Show(TableState state)
    {
        data = state;
        opponent.Show(data);
        effect.act = data.act;
        music.act = data.act;
        playing = data.phase != RoundPhase.Newspaper;
        foreach (GameObject panel in panels)
        {
            panel.SetActive(playing);
        }
        background.SetActive(playing);
        newspaperScreen.SetActive(!playing);
        roundResultText.gameObject.SetActive(playing);
        if (!playing)
        {
            gameCamera.backgroundColor = Color.black;
        }
        prediction = data.phase == RoundPhase.Prediction;
        table = data.phase == RoundPhase.ThreeCards || data.phase == RoundPhase.Shuffling ||
            data.phase == RoundPhase.Result && data.revealed;

        scoreText.gameObject.SetActive(playing);
        opponentCardText.gameObject.SetActive(playing);
        playerCardText.gameObject.SetActive(playing);
        tableCardText.gameObject.SetActive(playing);
        bankText.gameObject.SetActive(playing);
        suspicionText.gameObject.SetActive(playing);
        suspicionBar.SetActive(playing);
        roundResultText.text = data.message;
        nextRoundButton.gameObject.SetActive(playing && !prediction &&
            data.phase != RoundPhase.Shuffling && data.phase != RoundPhase.ThreeCards);

        nextLabel.text = "Следующий кон";
        if (data.phase == RoundPhase.Newspaper)
        {
            nextLabel.text = "Открыть газету";
        }
        else if (data.phase == RoundPhase.Ending)
        {
            nextLabel.text = "Начать заново";
        }
        else if (data.guestLeaving)
        {
            nextLabel.text = "Следующий гость";
        }

        lessButton.gameObject.SetActive(prediction);
        equalButton.gameObject.SetActive(prediction);
        greaterButton.gameObject.SetActive(prediction);
        lessButton.interactable = data.canGuess;
        equalButton.interactable = lessButton.interactable;
        greaterButton.interactable = lessButton.interactable;
        for (int i = 0; i < stakes.Length; i++)
        {
            active = playing && i < data.stakeCount;
            stakes[i].gameObject.SetActive(active);
            if (active)
            {
                amount = data.stakes[i];
                stakeLabels[i].text = amount.ToString("N0");
                if (data.allIn)
                {
                    stakeLabels[i].text = "Ва-банк " + amount.ToString("N0");
                }
                stakes[i].interactable = data.stakeAvailable[i];
                colors = stakes[i].colors;
                colors.normalColor = Color.white;
                colors.disabledColor = new Color(0.55f, 0.55f, 0.55f);
                if (data.selectedStake == i)
                {
                    colors.disabledColor = new Color(0.9f, 0.74f, 0.45f);
                }
                stakes[i].colors = colors;
            }
        }

        teaButton.gameObject.SetActive(prediction || data.phase == RoundPhase.ThreeCards);
        teaButton.interactable = data.canTea;
        bluffButton.gameObject.SetActive(prediction);
        bluffButton.interactable = data.canBluff;
        skipButton.gameObject.SetActive(prediction);
        skipButton.interactable = data.canSkip;
        sleeve.SetActive(data.showSleeve);

        for (int i = 0; i < tableCards.Length; i++)
        {
            tableCards[i].gameObject.SetActive(table);
            tableCards[i].interactable = data.phase == RoundPhase.ThreeCards;
            cardLabels[i].text = "?";
            if ((data.showing || data.revealed || data.holding &&
                data.seenCards[i] && data.teaLeft > 0))
            {
                cardLabels[i].text = "Пусто";
                if (i == data.correctCard)
                {
                    cardLabels[i].text = data.guestCard.ToString();
                }
            }
        }

        if (playing)
        {
            playerCardText.text = $"<size=26>Ваша карта</size>\n<size=64>{data.playerCard}</size>";
            opponentCardText.text = data.guestName + "\nКарта: ?";
            if (data.guestVisible || data.predictionsMade)
            {
                opponentCardText.text = data.guestName + $"\nКарта: {data.guestCard}";
            }
            sleeveText.text = $"Рукав: {data.sleeveCard}\nПеретащить на свою карту";
            scoreText.text = $"Золото: {data.gold:N0}\nАкт {data.act}  ·  Гость {data.guest}/{data.guestCount}  ·  Кон {data.guestRound}/{data.guestRounds}";
            bankText.text = $"Гость: {data.guestGold:N0}    Банк: {data.stake * 2:N0}";
            if (data.phase == RoundPhase.Result || data.phase == RoundPhase.Ending)
            {
                bankText.text = $"У гостя осталось: {data.guestGold:N0}";
            }
            tableCardText.text = "Моя карта относительно карты гостя";
            if (data.predictionsMade)
            {
                tableCardText.text = $"Вы: {signs[(int)data.playerPrediction]}    Гость: {signs[(int)data.guestPrediction]}";
            }
            gameCamera.backgroundColor = new Color(0.18f, 0.20f, 0.12f);
            if (data.gold <= 40000)
            {
                gameCamera.backgroundColor = new Color(0.13f, 0.15f, 0.14f);
            }
            if (data.act == 4)
            {
                gameCamera.backgroundColor = new Color(0.13f, 0.08f, 0.23f);
            }
            ShowRisk(data.suspicion, data.teaLeft, data.teaUsed);
        }
    }

    public void ShowRisk(float suspicion, float teaLeft, bool teaUsed)
    {
        suspicionText.text = $"Подозрение: {suspicion:0}%";
        teaLabel.text = "Чашка чая";
        if (teaLeft > 0)
        {
            teaLabel.text = $"Отвернулся: {teaLeft:0.0} с";
        }
        else if (teaUsed)
        {
            teaLabel.text = "Чашка потрачена";
        }
        suspicionFill.rectTransform.anchorMax = new Vector2(suspicion / 100f, 1);
        suspicionFill.color = Color.Lerp(new Color(0.79f, 0.65f, 0.36f), Color.red, suspicion / 100f);
    }
}
