using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ChickenOmelette.Core
{
    public class InputReader : MonoBehaviour
    {
        private GameInput input;

        public Vector2 Move => input.Player.Move.ReadValue<Vector2>();
        public bool FireHeld => input.Player.Fire.IsPressed();

        public event Action BombPerformed;
        public event Action PausePerformed;

        private void Awake()
        {
            input = new GameInput();
        }

        private void OnEnable()
        {
            input.Player.Enable();
            input.Player.Bomb.performed += OnBomb;
            input.Player.Pause.performed += OnPause;
        }

        private void OnDisable()
        {
            input.Player.Bomb.performed -= OnBomb;
            input.Player.Pause.performed -= OnPause;
            input.Player.Disable();
        }

        private void OnDestroy()
        {
            input.Dispose();
        }

        private void OnBomb(InputAction.CallbackContext context)
        {
            BombPerformed?.Invoke();
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            PausePerformed?.Invoke();
        }
    }
}