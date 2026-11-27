using UnityEngine;
using UnityEngine.UI;

public class OpponentView : MonoBehaviour
{
    public RawImage portrait;
    private bool visible;

    public void Show(TableState data)
    {
        visible = data.hasPortrait && data.phase != RoundPhase.Newspaper &&
            data.phase != RoundPhase.Ending;
        portrait.gameObject.SetActive(visible);
        if (visible)
        {
            portrait.texture = data.withoutCard;
            if (data.phase == RoundPhase.Prediction)
            {
                portrait.texture = data.withCard;
            }
        }
    }
}
