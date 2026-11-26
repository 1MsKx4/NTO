using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RoundController game;
    public RectTransform playerCard;
    public Canvas canvas;
    public int card;
    private Vector2 home;
    private RectTransform rect;
    private bool pressed, dragging, cheatClick, dropped;
    private float pressedSeconds;
    private Bounds cardBounds;

    private void Awake()
    {
        rect = (RectTransform)transform;
        home = rect.anchoredPosition;
    }

    private void OnEnable()
    {
        pressed = false;
        dragging = false;
        cheatClick = false;
        pressedSeconds = 0;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            pressed = true;
            pressedSeconds = 0;
            cheatClick = game.TeaActive();
            if (card < 0 || cheatClick)
            {
                game.HoldCard(card);
            }
        }
    }

    private void Update()
    {
        if (pressed && card >= 0 && !cheatClick)
        {
            pressedSeconds += Time.deltaTime;
            if (pressedSeconds >= game.PeekSeconds())
            {
                cheatClick = true;
                game.HoldCard(card);
            }
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (pressed && data.button == PointerEventData.InputButton.Left)
        {
            if (dragging)
            {
                Drop();
            }
            else
            {
                pressed = false;
                if (card >= 0 && !cheatClick)
                {
                    game.ChooseCard(card);
                }
                else
                {
                    game.ReleaseCard(false);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (card == -2 && data.button == PointerEventData.InputButton.Left)
        {
            dragging = game.HoldCard(card);
            pressed = dragging;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (dragging)
        {
            rect.anchoredPosition += data.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (dragging)
        {
            Drop();
        }
    }

    private void Drop()
    {
        dragging = false;
        pressed = false;
        cardBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(playerCard, rect);
        dropped = playerCard.rect.Overlaps(new Rect(cardBounds.min, cardBounds.size));
        rect.anchoredPosition = home;
        game.ReleaseCard(dropped);
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused && pressed)
        {
            pressed = false;
            dragging = false;
            rect.anchoredPosition = home;
            game.ReleaseCard(false);
        }
    }

    private void OnDisable()
    {
        if (pressed)
        {
            game.CancelHold();
        }
        pressed = false;
        dragging = false;
        rect.anchoredPosition = home;
    }
}
