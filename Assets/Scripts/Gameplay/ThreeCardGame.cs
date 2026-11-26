using System.Collections;
using UnityEngine;

public class ThreeCardGame : MonoBehaviour
{
    public int correctCard, chosenCard;
    public bool showing, revealed;
    public bool changed, ready;
    public int first, second, pass;
    public float progress;
    public bool swapping;
    private float elapsed, duration;
    private float showSeconds;
    private int moves;

    public void ResetRound()
    {
        StopAllCoroutines();
        showing = false;
        revealed = false;
        changed = false;
        ready = false;
        swapping = false;
    }

    public void ClearChange()
    {
        changed = false;
    }

    public bool Reveal(int index)
    {
        chosenCard = index;
        revealed = true;
        return index == correctCard;
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
            elapsed = 0;
            progress = 0;
            swapping = true;
            yield return null;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                progress = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            swapping = false;
            yield return new WaitForSeconds(0.1f);
        }
        ready = true;
        changed = true;
    }
}
