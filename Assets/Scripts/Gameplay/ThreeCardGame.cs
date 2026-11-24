using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThreeCardGame : MonoBehaviour
{
    public Button[] cards;
    public int correctCard, chosenCard;
    public bool showing, revealed;
    public bool changed, ready;
    private Vector2[] places = new Vector2[3];
    private int first, second, pass;
    private float elapsed, duration;
    private float showSeconds;
    private int moves;
    private Vector2 firstPlace, secondPlace;
    private RectTransform firstRect, secondRect;

    private void Awake()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            places[i] = ((RectTransform)cards[i].transform).anchoredPosition;
        }
    }

    public void Begin(float seconds, float shuffleSeconds, int shuffleMoves)
    {
        showSeconds = seconds;
        duration = shuffleSeconds;
        moves = shuffleMoves;
        ready = false;
        changed = true;
        correctCard = Random.Range(0, 3);
        chosenCard = -1;
        showing = true;
        revealed = false;
        for (int i = 0; i < cards.Length; i++)
        {
            ((RectTransform)cards[i].transform).anchoredPosition = places[i];
        }
        StartCoroutine(Shuffle());
    }

    private IEnumerator Shuffle()
    {
        yield return new WaitForSeconds(showSeconds);
        showing = false;
        changed = true;
        for (pass = 0; pass < moves; pass++)
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
        ready = true;
        changed = true;
    }
}
