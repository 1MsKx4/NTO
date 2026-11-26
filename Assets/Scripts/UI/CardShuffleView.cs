using UnityEngine;

public class CardShuffleView : MonoBehaviour
{
    public RectTransform[] cards;
    private Vector2[] places = new Vector2[3];
    private Vector2 firstPlace, secondPlace;
    private int lastPass;

    private void Awake()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            places[i] = cards[i].anchoredPosition;
        }
    }

    public void ResetCards()
    {
        lastPass = -1;
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].anchoredPosition = places[i];
        }
    }

    public void MoveCards(int pass, int first, int second, float progress)
    {
        if (lastPass != pass)
        {
            lastPass = pass;
            firstPlace = cards[first].anchoredPosition;
            secondPlace = cards[second].anchoredPosition;
        }
        cards[first].anchoredPosition = Vector2.Lerp(firstPlace, secondPlace, progress);
        cards[second].anchoredPosition = Vector2.Lerp(secondPlace, firstPlace, progress);
    }
}
