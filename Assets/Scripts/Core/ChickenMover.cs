using UnityEngine;

namespace ChickenOmelette.Core
{
    public class ChickenMover : MonoBehaviour, IPoolable
    {
        public enum State
        {
            Idle,
            Entering,
            InFormation,
            Diving,
            Returning
        }

        [Tooltip("How far above the top edge a diving chicken re-appears before flying back to its slot.")]
        [SerializeField, Min(0f)] private float returnMargin = 1.5f;

        private State state;
        private FormationController formation;
        private int slotIndex;
        private EntryPath path;
        private float speed;
        private float diveSpeed;
        private float distance;

        public State CurrentState => state;
        public bool IsInFormation => state == State.InFormation;
        public bool IsDiving => state == State.Diving || state == State.Returning;
        public int SlotIndex => slotIndex;

        public void BeginEntry(FormationController owner, int slot, EntryPath entryPath, float moveSpeed)
        {
            formation = owner;
            slotIndex = slot;
            path = entryPath;
            speed = moveSpeed;
            distance = 0f;
            transform.position = path.Evaluate(0f);
            state = State.Entering;
            formation.Register(this);
        }

        public bool BeginDive(EntryPath divePath, float speedAlongPath)
        {
            if (state != State.InFormation)
            {
                return false;
            }

            path = divePath;
            diveSpeed = speedAlongPath;
            distance = 0f;
            state = State.Diving;
            return true;
        }

        public void OnSpawn()
        {
            state = State.Idle;
        }

        public void OnDespawn()
        {
            if (formation != null)
            {
                formation.Unregister(this);
            }

            formation = null;
            state = State.Idle;
        }

        private void Update()
        {
            switch (state)
            {
                case State.Entering:
                    UpdateEntering();
                    break;
                case State.InFormation:
                    transform.position = formation.GetSlotPosition(slotIndex);
                    break;
                case State.Diving:
                    UpdateDiving();
                    break;
                case State.Returning:
                    UpdateReturning();
                    break;
            }
        }

        private void UpdateEntering()
        {
            float step = speed * Time.deltaTime;
            distance += step;

            if (distance < path.Length)
            {
                transform.position = path.Evaluate(distance);
                return;
            }

            Vector3 target = formation.GetSlotPosition(slotIndex);
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            if ((transform.position - target).sqrMagnitude < 0.0004f)
            {
                state = State.InFormation;
            }
        }

        private void UpdateDiving()
        {
            distance += diveSpeed * Time.deltaTime;

            if (distance < path.Length)
            {
                transform.position = path.Evaluate(distance);
                return;
            }

            Vector3 slot = formation.GetSlotPosition(slotIndex);
            transform.position = new Vector3(slot.x, ScreenBounds.Top + returnMargin, transform.position.z);
            state = State.Returning;
        }

        private void UpdateReturning()
        {
            float step = speed * Time.deltaTime;
            Vector3 target = formation.GetSlotPosition(slotIndex);
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            if ((transform.position - target).sqrMagnitude < 0.0004f)
            {
                state = State.InFormation;
            }
        }
    }
}