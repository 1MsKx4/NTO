using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThreeCardGame : MonoBehaviour
{
    public RoundController game;
    public Button[] cards;
    public int correctCard, chosenCard;
    public bool showing, revealed;
    private Vector2[] places = new Vector2[3];
    private int first, second, pass;
    private float elapsed, duration;
    private Vector2 firstPlace, secondPlace;
    private RectTransform firstRect, secondRect;

    private void Awake()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            places[i] = ((RectTransform)cards[i].transform).anchoredPosition;
        }
    }

    public void Begin()
    {
        correctCard = Random.Range(0, 3);
        chosenCard = -1;
        showing = true;
        revealed = false;
        for (int i = 0; i < cards.Length; i++)
        {
            ((RectTransform)cards[i].transform).anchoredPosition = places[i];
        }
        game.view.Show();
        StartCoroutine(Shuffle());
    }

    private IEnumerator Shuffle()
    {
        yield return new WaitForSeconds(game.match.settings.showCardSeconds);
        showing = false;
        game.view.Show();
        duration = game.match.GetAct().shuffleSeconds;
        for (pass = 0; pass < game.match.GetAct().shuffleMoves; pass++)
        {
            first = Random.Range(0, 3);
            second = (first + Random.Range(1, 3)) % 3;
            firstRect = (RectTransform)cards[first].transform;
            secondRect = (RectTransform)cards[second].transform;
            firstPlace = firstRect.anchoredPosition;
            secondPlace = secondRect.anchoredPosition;
            elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                firstRect.anchoredPosition = Vector2.Lerp(firstPlace, secondPlace, elapsed / duration);
                secondRect.anchoredPosition = Vector2.Lerp(secondPlace, firstPlace, elapsed / duration);
                yield return null;
            }
            firstRect.anchoredPosition = secondPlace;
            secondRect.anchoredPosition = firstPlace;
            yield return new WaitForSeconds(0.1f);
        }
        game.phase = RoundPhase.ThreeCards;
        game.message = "Найдите карту гостя среди трёх. Для подсмотра сначала используйте чашку.";
        game.view.Show();
    }
}
