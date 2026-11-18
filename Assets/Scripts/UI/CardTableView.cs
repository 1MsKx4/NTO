using UnityEngine;
using UnityEngine.UI;

public class CardTableView : MonoBehaviour
{
    public Text playerCardText, opponentCardText, tableCardText, roundResultText, scoreText;
    public Button lessButton, equalButton, greaterButton, nextRoundButton, skipButton;
    public Button[] stakes, tricks;
    public RoundResolver roundResolver;
    public MatchScore matchScore;
    public Camera gameCamera;

    private CardDeck deck = new CardDeck();
    private Text suspicionText;
    private Image suspicionFill;
    private string[] trickNames = { "Чай", "Подглядеть", "Крап", "Тасовка" };
    private int openCard, nextCard, stake, selectedStake, suspicion, teaCount, markedCard;
    private float teaUntil;
    private bool roundOpen, started, skipped, tutorialPlayed;

    private void Start()
    {
        lessButton.onClick.AddListener(() => Guess(Prediction.Less));
        equalButton.onClick.AddListener(() => Guess(Prediction.Equal));
        greaterButton.onClick.AddListener(() => Guess(Prediction.Greater));
        nextRoundButton.onClick.AddListener(Continue);

        for (int i = 0; i < stakes.Length; i++)
        {
            int index = i;
            stakes[i].onClick.AddListener(() => SelectStake(index));
        }
        for (int i = 0; i < tricks.Length; i++)
        {
            int index = i;
            tricks[i].onClick.AddListener(() => Cheat(index));
        }
        skipButton.onClick.AddListener(Skip);

        Fit(playerCardText, 680);
        Fit(opponentCardText, 300);
        playerCardText.rectTransform.anchoredPosition = new Vector2(0, 250);
        opponentCardText.rectTransform.anchoredPosition = new Vector2(570, 285);
        suspicionText = Instantiate(opponentCardText, opponentCardText.transform.parent);
        suspicionText.name = "Suspicion";
        suspicionText.rectTransform.anchoredPosition = new Vector2(570, 224);
        suspicionText.raycastTarget = false;

        Image track = new GameObject("Suspicion Bar", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        track.transform.SetParent(opponentCardText.transform.parent, false);
        track.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        track.rectTransform.anchorMax = track.rectTransform.anchorMin;
        track.rectTransform.anchoredPosition = new Vector2(570, 179);
        track.rectTransform.sizeDelta = new Vector2(174, 8);
        track.color = new Color(0.24f, 0.24f, 0.24f);
        track.raycastTarget = false;
        suspicionFill = new GameObject("Fill", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        suspicionFill.transform.SetParent(track.transform, false);
        suspicionFill.rectTransform.anchorMin = Vector2.zero;
        suspicionFill.rectTransform.anchorMax = Vector2.up;
        suspicionFill.rectTransform.offsetMin = Vector2.zero;
        suspicionFill.rectTransform.offsetMax = Vector2.zero;
        suspicionFill.raycastTarget = false;

        tableCardText.rectTransform.anchoredPosition = new Vector2(0, 122);
        roundResultText.rectTransform.anchoredPosition = new Vector2(0, -320);
        Fit(roundResultText, 1000);
        Fit(scoreText, 1100);
        ShowNewspaper();
    }

    private void Fit(Text label, int width)
    {
        label.rectTransform.sizeDelta = new Vector2(width, 72);
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 18;
        label.resizeTextMaxSize = 32;
    }

    private void ShowNewspaper()
    {
        started = false;
        roundOpen = false;
        playerCardText.text = "";
        opponentCardText.text = "";
        tableCardText.text = "";
        scoreText.text = "";
        roundResultText.text = "Газета: «Граф желает играть»";
        nextRoundButton.GetComponentInChildren<Text>().text = "Открыть газету";
        RefreshButtons();
    }

    private void Continue()
    {
        if (!started || matchScore.Finished)
        {
            if (!matchScore.ResetMatch())
            {
                return;
            }
            deck.Shuffle();
            started = true;
            skipped = false;
            tutorialPlayed = false;
            markedCard = 0;
        }
        else
        {
            matchScore.NextGuest();
        }

        openCard = deck.Draw();
        stake = 0;
        selectedStake = -1;
        suspicion = 0;
        teaCount = 0;
        teaUntil = 0;
        roundOpen = true;
        tableCardText.text = $"Открытая карта: {openCard}";
        roundResultText.text = "Выберите ставку и угадайте следующую карту";
        nextRoundButton.GetComponentInChildren<Text>().text = "Следующая раздача";
        ShowTable();
        RefreshButtons();
    }

    private void ShowTable()
    {
        int gold = matchScore.Gold;
        int start = matchScore.settings.startingGold;
        string room = "Золото и книги";
        Color tint = new Color(0.18f, 0.20f, 0.12f);
        if (gold * 3L < start * 2L)
        {
            room = "Половина богатств исчезла";
            tint = new Color(0.13f, 0.15f, 0.14f);
        }
        if (gold * 4L < start)
        {
            room = $"Граф считает остаток: {gold:N0}";
            tint = new Color(0.08f, 0.09f, 0.10f);
        }

        if (matchScore.Act == 4)
        {
            tint = new Color(0.13f, 0.08f, 0.23f);
        }

        gameCamera.backgroundColor = tint;
        playerCardText.text = teaUntil > Time.time ? "Гость смотрит на чашку" : room;
        opponentCardText.text = matchScore.CurrentGuest.guestName;
        scoreText.text = $"Золото: {gold:N0}    Акт: {matchScore.Act}    Гость: {matchScore.Guest}/{matchScore.CurrentAct.guests.Length}    Раундов: {matchScore.Rounds}";
        suspicionText.text = $"Подозрение: {suspicion}%";
        suspicionFill.rectTransform.anchorMax = new Vector2(suspicion / 100f, 1f);
        suspicionFill.color = Color.Lerp(new Color(0.79f, 0.65f, 0.36f), new Color(0.85f, 0.27f, 0.24f), suspicion / 100f);
    }

    private void Update()
    {
        if (roundOpen && teaUntil > 0 && Time.time >= teaUntil)
        {
            teaUntil = 0;
            ShowTable();
            RefreshButtons();
        }
    }

    private void RefreshButtons()
    {
        bool canGuess = roundOpen && (stake > 0 || matchScore.Act == 4 && matchScore.Gold == 0);
        lessButton.interactable = canGuess;
        equalButton.interactable = canGuess;
        greaterButton.interactable = canGuess;
        nextRoundButton.interactable = !roundOpen;
        lessButton.gameObject.SetActive(started);
        equalButton.gameObject.SetActive(started);
        greaterButton.gameObject.SetActive(started);
        if (started)
        {
            equalButton.GetComponentInChildren<Text>(true).text = $"Равно  =   ×{matchScore.settings.equalMultiplier}";
        }
        suspicionText.gameObject.SetActive(started);
        suspicionFill.transform.parent.gameObject.SetActive(started);

        for (int i = 0; i < stakes.Length; i++)
        {
            int amount = started ? matchScore.GetStake(i) : 0;
            stakes[i].gameObject.SetActive(started);
            stakes[i].interactable = roundOpen && amount > 0 && (i != 0 || !matchScore.LowGold());
            stakes[i].GetComponentInChildren<Text>(true).text = amount.ToString("N0");
            ColorBlock colors = stakes[i].colors;
            colors.normalColor = i == selectedStake ? new Color(0.89f, 0.74f, 0.45f) : Color.white;
            colors.selectedColor = colors.normalColor;
            stakes[i].colors = colors;
        }
        for (int i = 0; i < tricks.Length; i++)
        {
            bool unlocked = started && (i < 2 || matchScore.Act >= 2);
            tricks[i].gameObject.SetActive(unlocked);
            tricks[i].interactable = roundOpen && tutorialPlayed && stake > 0;
            int risk = GetRisk(i);
            tricks[i].GetComponentInChildren<Text>(true).text = $"{trickNames[i]}\n+{risk}%";
        }
        skipButton.gameObject.SetActive(started);
        skipButton.interactable = roundOpen && tutorialPlayed && !skipped && !matchScore.LowGold();
    }

    private void SelectStake(int index)
    {
        stake = matchScore.GetStake(index);
        selectedStake = index;
        roundResultText.text = $"Ставка: {stake:N0}. Выберите <, = или >";
        RefreshButtons();
    }

    private void Guess(Prediction prediction)
    {
        if (!roundOpen)
        {
            return;
        }

        nextCard = deck.Draw();
        tableCardText.text = $"{openCard}  →  {nextCard}";
        if (!roundResolver.Resolve(openCard, nextCard, prediction))
        {
            FinishRound(-stake, "Промах");
            return;
        }

        int prize = stake;
        if (prediction == Prediction.Equal)
        {
            prize *= matchScore.settings.equalMultiplier;
        }
        if (nextCard == 1 || nextCard == 12)
        {
            prize += (int)((long)prize * matchScore.settings.edgeBonus / 100);
        }
        FinishRound(prize, "Угадали");
    }

    private void Cheat(int trick)
    {
        int risk = GetRisk(trick);
        string message;
        if (trick == 0)
        {
            teaCount++;
            teaUntil = Time.time + matchScore.settings.teaSeconds;
            message = $"Гость отвлечён на {matchScore.settings.teaSeconds:0.#} с";
        }
        else
        {
            if (trick == 1)
            {
                message = $"Подглядели. Следующая карта: {deck.Peek()}";
            }
            else if (trick == 2)
            {
                markedCard = deck.Peek();
                message = $"Пометили карту {markedCard}";
            }
            else
            {
                deck.MixRemaining();
                message = "Оставшиеся карты перемешаны";
            }
        }

        suspicion = Mathf.Min(100, suspicion + risk);
        if (suspicion == 100)
        {
            markedCard = 0;
            int fine = matchScore.CurrentAct.fine;
            FinishRound(-stake - fine, "Поймали. Ставка сгорела");
            return;
        }

        if (markedCard != 0 && trick == 3 && deck.Peek() == markedCard)
        {
            message += $". Сверху крап: {markedCard}";
        }
        roundResultText.text = message;
        ShowTable();
        RefreshButtons();
    }

    private int GetRisk(int trick)
    {
        if (!started)
        {
            return 0;
        }
        if (trick == 0)
        {
            return teaCount == 0 ? 0 : matchScore.settings.repeatTeaRisk;
        }
        int risk = matchScore.settings.normalRisk;
        if (teaUntil > Time.time)
        {
            risk = matchScore.settings.distractedRisk;
        }
        return Mathf.Min(100, risk + matchScore.CurrentGuest.extraSuspicion);
    }

    private void Skip()
    {
        skipped = true;
        nextCard = deck.Draw();
        tableCardText.text = $"{openCard}  →  {nextCard}";
        FinishRound(0, "Раздача пропущена");
    }

    private void FinishRound(int change, string message)
    {
        roundOpen = false;
        teaUntil = 0;
        tutorialPlayed = true;
        int oldGold = matchScore.Gold;
        matchScore.AddRound(change);
        int difference = matchScore.Gold - oldGold;
        roundResultText.text = $"{message}. Золото: {difference:+#;-#;0}";
        if (matchScore.Finished)
        {
            roundResultText.text = matchScore.Ending;
            nextRoundButton.GetComponentInChildren<Text>().text = "Начать заново";
        }
        ShowTable();
        RefreshButtons();
    }
}
