using UnityEngine;
using UnityEngine.UI;

public class BackgroundEffect : MonoBehaviour
{
    public MatchScore match;
    public Image background;
    public RectTransform picture;
    public Color finalColor = new Color(0.65f, 0.48f, 0.85f);
    public float speed = 0.7f, stretch = 0.015f, tilt = 0.6f;
    private Vector3 normalScale;
    private Quaternion normalRotation;
    private Color normalColor;
    private float wave;

    private void Awake()
    {
        normalScale = picture.localScale;
        normalRotation = picture.localRotation;
        normalColor = background.color;
    }

    private void Update()
    {
        if (match.Act == 4)
        {
            wave = Mathf.Sin(Time.time * speed);
            picture.localScale = Vector3.Scale(normalScale,
                new Vector3(1.04f + wave * stretch, 1.04f - wave * stretch, 1));
            picture.localRotation = normalRotation * Quaternion.Euler(0, 0, wave * tilt);
            background.color = finalColor;
        }
        else
        {
            picture.localScale = normalScale;
            picture.localRotation = normalRotation;
            background.color = normalColor;
        }
    }

    private void OnDisable()
    {
        picture.localScale = normalScale;
        picture.localRotation = normalRotation;
        background.color = normalColor;
    }
}
