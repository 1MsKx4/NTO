using UnityEngine;

public class CardDeck
{
    private int[] cards = new int[24];
    private int index;

    public void Shuffle()
    {
        index = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = i % 12 + 1;
        }
        MixRemaining();
    }

    public void MixRemaining()
    {
        for (int i = cards.Length - 1; i > index; i--)
        {
            int randomIndex = Random.Range(index, i + 1);
            int card = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = card;
        }
    }

    public int Draw()
    {
        int card = Peek();
        index++;
        return card;
    }

    public int Peek()
    {
        if (index == cards.Length)
        {
            Shuffle();
        }
        return cards[index];
    }
}
