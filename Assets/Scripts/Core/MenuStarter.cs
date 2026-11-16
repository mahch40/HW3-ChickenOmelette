using UnityEngine;
using UnityEngine.InputSystem;

namespace ChickenOmelette.Core
{
    public class MenuStarter : MonoBehaviour
    {
        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || GameManager.Instance == null)
            {
                return;
            }

            if (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartGame();
            }
        }
    }
}