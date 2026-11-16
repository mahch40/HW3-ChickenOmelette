using UnityEngine;

namespace ChickenOmelette.Core
{
    public class RunStateApplier : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameObject player;

        private void Start()
        {
            GameManager manager = GameManager.Instance;
            if (manager == null || !manager.Checkpoint.hasCarry)
            {
                return;
            }

            RunState state = manager.Checkpoint;

            if (scoreManager != null)
            {
                scoreManager.SetScore(state.score);
            }

            if (player == null)
            {
                return;
            }

            if (player.TryGetComponent(out PlayerHealth health))
            {
                health.SetLives(state.lives);
            }

            if (player.TryGetComponent(out PlayerPower power))
            {
                power.SetLevel(state.powerLevel);
            }

            if (player.TryGetComponent(out MissileLauncher launcher))
            {
                launcher.SetCount(state.missiles);
            }
        }
    }
}