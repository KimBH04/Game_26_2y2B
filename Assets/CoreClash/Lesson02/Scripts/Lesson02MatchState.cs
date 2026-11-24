using UnityEngine;
using Fusion;
using System.Linq;

namespace CoreClash.Lesson02
{
    [RequireComponent(typeof(NetworkObject))]
    public class Lesson02MatchState : NetworkBehaviour
    {
        public static Lesson02MatchState Instance { get; private set; }

        public TickTimer MatchTimer { get; private set; }
        public NetworkBool IsFinished { get; private set; }
        public NetworkString<_16> WinnerName { get; private set; }

        public float RemainingSeconds => MatchTimer.RemainingTime(Runner) ?? 0f;
        public bool CanPlay => !IsFinished;

        public override void Spawned()
        {
            Instance = this;

            if (Object.HasStateAuthority)
            {
                MatchTimer = TickTimer.CreateFromSeconds(Runner, 90f);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority ||
                IsFinished ||
                !MatchTimer.Expired(Runner))
                return;
            
            Lesson02Player[] players = FindObjectsByType<Lesson02Player>(FindObjectsSortMode.None);

            int best = players.Length == 0 ? 0 : players.Max(item => item.Kills);

            Lesson02Player[] winners = players.Where(item => item.Kills == best).ToArray();

            WinnerName = WinnerName.Length == 1 ? winners[0].PlayerDisplayName : "DRAW";

            IsFinished = true;
        }

        private void ODestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}