using UnityEngine;
using UnityEngine.UI;

public class CardTableView : MonoBehaviour
{
    public Text playerCardText, opponentCardText, tableCardText, roundResultText;
    public Button lessButton, equalButton, greaterButton, nextRoundButton;
    public RoundResolver roundResolver;
    public MatchScore matchScore;
    private int playerCard, opponentCard, tableCard;
    private int[] deck = new int[12];
    private Button[] tableButtons = new Button[12];
    private Text[] tableLabels = new Text[12];
    private string[] signs = { "<", "=", ">" };
    private Prediction playerPrediction, opponentPrediction;
    private bool playerChoosesCard = true, waitingForCard;
    private RoundOutcome outcome;
    private string result;

    private void Start()
    {
        for (int i = 0; i < tableButtons.Length; i++)
        {
            int index = i;
            tableButtons[i] = Instantiate(nextRoundButton, nextRoundButton.transform.parent);
            tableButtons[i].name = "Table Card " + (i + 1);
            RectTransform card = (RectTransform)tableButtons[i].transform;
            card.sizeDelta = new Vector2(80, 80);
            card.anchoredPosition = new Vector2(i % 6 * 100 - 250, 255 - i / 6 * 100);
            tableLabels[i] = tableButtons[i].GetComponentInChildren<Text>();
            tableButtons[i].onClick.AddListener(() => ChooseCard(index));
        }

        lessButton.onClick.AddListener(PredictLess);
        equalButton.onClick.AddListener(PredictEqual);
        greaterButton.onClick.AddListener(PredictGreater);
        nextRoundButton.onClick.AddListener(BeginRound);
        BeginRound();
    }

    private void PredictLess()
    {
        SubmitPrediction(Prediction.Less);
    }

    private void PredictEqual()
    {
        SubmitPrediction(Prediction.Equal);
    }

    private void PredictGreater()
    {
        SubmitPrediction(Prediction.Greater);
    }

    private void BeginRound()
    {
        int randomIndex, savedCard;

        playerCard = Random.Range(1, 13);
        opponentCard = Random.Range(1, 12);
        if (opponentCard >= playerCard)
        {
            opponentCard++;
        }
        opponentPrediction = (Prediction)Random.Range(0, 3);

        for (int i = 0; i < deck.Length; i++)
        {
            deck[i] = i + 1;
            tableButtons[i].interactable = false;
            tableLabels[i].text = "?";
        }
        for (int i = deck.Length - 1; i > 0; i--)
        {
            randomIndex = Random.Range(0, i + 1);
            savedCard = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = savedCard;
        }

        playerCardText.text = $"Ваша карта: {playerCard}";
        opponentCardText.text = "Карта противника: ?";
        tableCardText.text = "Карта на столе: ?";
        roundResultText.text = "Ваша карта меньше, равна или больше карты стола?";
        waitingForCard = false;
        lessButton.interactable = true;
        equalButton.interactable = true;
        greaterButton.interactable = true;
        nextRoundButton.interactable = false;
    }

    private void SubmitPrediction(Prediction prediction)
    {
        playerPrediction = prediction;
        lessButton.interactable = false;
        equalButton.interactable = false;
        greaterButton.interactable = false;
        waitingForCard = true;

        if (!playerChoosesCard)
        {
            ChooseCard(Random.Range(0, deck.Length));
            return;
        }

        roundResultText.text = "Выберите закрытую карту на столе";
        for (int i = 0; i < tableButtons.Length; i++)
        {
            tableButtons[i].interactable = true;
        }
    }

    private void ChooseCard(int index)
    {
        if (!waitingForCard)
        {
            return;
        }

        waitingForCard = false;
        tableCard = deck[index];
        tableLabels[index].text = tableCard.ToString();
        for (int i = 0; i < tableButtons.Length; i++)
        {
            tableButtons[i].interactable = false;
        }

        outcome = roundResolver.Resolve(
            playerCard, playerPrediction, opponentCard, opponentPrediction, tableCard);
        matchScore.AddRound(outcome);
        opponentCardText.text = $"Карта противника: {opponentCard}";
        tableCardText.text = $"Карта на столе: {tableCard}";
        result = "Ничья";
        if (outcome == RoundOutcome.PlayerWins)
        {
            result = "Вы выиграли";
            playerChoosesCard = true;
        }
        else if (outcome == RoundOutcome.OpponentWins)
        {
            result = "Противник выиграл";
            playerChoosesCard = false;
        }
        roundResultText.text = $"{result} · Вы: {signs[(int)playerPrediction]} · Противник: {signs[(int)opponentPrediction]}";
        nextRoundButton.interactable = true;
    }
}
