using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menuScreen;
    public RoundController game;

    public void Play()
    {
        game.Newspaper();
        menuScreen.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
