using UnityEngine;

namespace ChickenOmelette.Core
{
    [RequireComponent(typeof(Rigidbody2D), typeof(InputReader))]
    public class ShipController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 9f;
        [Tooltip("Distance kept between the ship centre and the screen edges.")]
        [SerializeField] private Vector2 screenPadding = new Vector2(0.5f, 0.7f);

        private Rigidbody2D body;
        private InputReader input;
        private Vector2 moveInput;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            input = GetComponent<InputReader>();
        }

        private void Update()
        {
            moveInput = Vector2.ClampMagnitude(input.Move, 1f);
        }

        private void FixedUpdate()
        {
            Vector2 target = body.position + moveInput * (moveSpeed * Time.fixedDeltaTime);
            body.MovePosition(ScreenBounds.Clamp(target, screenPadding));
        }
    }
}