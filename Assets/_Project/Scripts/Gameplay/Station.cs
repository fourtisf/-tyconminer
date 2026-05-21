using System;
using GoldMineTycoon.Data;

namespace GoldMineTycoon.Gameplay
{
    /// <summary>
    /// Runtime state for one production station. This is plain C# (not a MonoBehaviour)
    /// so production math runs deterministically and headlessly for offline calculation
    /// and unit tests. A separate StationView MonoBehaviour mirrors this for visuals.
    /// </summary>
    public class Station
    {
        public StationData Data { get; }
        public int Level { get; private set; }
        public int Stored { get; private set; }

        private float _cycleProgress; // seconds accumulated toward the current cycle

        public Station(StationData data, int level, int stored)
        {
            Data = data;
            Level = Math.Max(1, level);
            Stored = stored;
        }

        public int Capacity => Data.CapacityAtLevel(Level);
        public bool IsFull => Stored >= Capacity;
        public double UpgradeCost => Data.UpgradeCost(Level);

        public void Upgrade() => Level++;

        /// <summary>
        /// Advance production. Returns items completed this tick (0 if stalled/full).
        /// speedMultiplier &lt; 1 means faster (manager buff); outputMultiplier scales yield.
        /// </summary>
        public int Tick(float deltaSeconds, float speedMultiplier, float outputMultiplier)
        {
            if (IsFull) return 0;

            var cycleLength = Data.baseCycleSeconds * speedMultiplier;
            if (cycleLength <= 0f) return 0;

            _cycleProgress += deltaSeconds;
            var completed = 0;
            while (_cycleProgress >= cycleLength && !IsFull)
            {
                _cycleProgress -= cycleLength;
                var yield = (int)Math.Round(Data.OutputAtLevel(Level) * outputMultiplier);
                var room = Capacity - Stored;
                var added = Math.Min(Math.Max(1, yield), room);
                Stored += added;
                completed += added;
            }
            return completed;
        }

        /// <summary>Remove up to <paramref name="count"/> items (consumed by next station or shipped).</summary>
        public int Withdraw(int count)
        {
            var taken = Math.Min(count, Stored);
            Stored -= taken;
            return taken;
        }

        public void Deposit(int count) => Stored = Math.Min(Capacity, Stored + count);
    }
}
