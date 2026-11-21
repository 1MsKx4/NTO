using UnityEngine;
using UnityEngine.InputSystem;

public class RecordingControls : MonoBehaviour
{
    public RoundController game;
    public bool hotkeysEnabled = true;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void Update()
    {
        if (hotkeysEnabled)
        {
            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                game.RecordingGold(35000);
            }
            else if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                game.RecordingGold(20000);
            }
            else if (Keyboard.current.f3Key.wasPressedThisFrame)
            {
                game.RecordingGold(10000);
            }
            else if (Keyboard.current.f4Key.wasPressedThisFrame)
            {
                game.RecordingGold(0);
            }
            else if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                game.RecordingAct(1);
            }
            else if (Keyboard.current.f6Key.wasPressedThisFrame)
            {
                game.RecordingAct(2);
            }
            else if (Keyboard.current.f7Key.wasPressedThisFrame)
            {
                game.RecordingAct(3);
            }
            else if (Keyboard.current.f8Key.wasPressedThisFrame)
            {
                game.RecordingAct(4);
            }
        }
    }
#endif
}
