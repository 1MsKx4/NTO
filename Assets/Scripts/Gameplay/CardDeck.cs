using UnityEngine;

public class CardDeck
{
    private int[] cards = new int[12];
    private int index, randomIndex, saved;

    public void Shuffle()
    {
        index = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = i + 1;
        }
        for (int i = cards.Length - 1; i > 0; i--)
        {
            randomIndex = Random.Range(0, i + 1);
            saved = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = saved;
        }
    }

    public int Draw()
    {
        if (index == cards.Length)
        {
            Shuffle();
        }
        index++;
        return cards[index - 1];
    }
}
