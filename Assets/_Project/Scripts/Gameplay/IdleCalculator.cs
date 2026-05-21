using System;
using GoldMineTycoon.Core;
using GoldMineTycoon.Data;

namespace GoldMineTycoon.Gameplay
{
    /// <summary>
    /// Computes cash earned while the app was closed. Only manager-run Shipping
    /// stations earn offline (the idle promise). Offline time is capped so a player
    /// gone for a week doesn't get unbounded cash, and an offline multiplier (&lt;1)
    /// keeps active play more rewarding than idling.
    /// </summary>
    public static class IdleCalculator
    {
        public const long MaxOfflineSeconds = 8 * 60 * 60; // cap at 8 hours
        public const double OfflineEfficiency = 0.5;        // earn 50% of active rate

        public static double CalculateOfflineEarnings(
            SaveData save, ProductionSystem production, ManagerService managers, long offlineSeconds)
        {
            var seconds = Math.Min(offlineSeconds, MaxOfflineSeconds);
            if (seconds <= 0) return 0;

            double total = 0;
            foreach (var station in production.Stations)
            {
                if (station.Data.kind != StationKind.Shipping) continue;
                if (!managers.HasManagerForStation(station.Data.id)) continue;

                var speed = managers.SpeedMultiplierFor(station.Data.id);
                var output = managers.OutputMultiplierFor(station.Data.id);
                var cycleLength = station.Data.baseCycleSeconds * speed;
                if (cycleLength <= 0) continue;

                var cycles = seconds / cycleLength;
                var perCycle = station.Data.OutputAtLevel(station.Level) * output;
                total += cycles * perCycle * station.Data.cashPerItem;
            }
            return total * OfflineEfficiency;
        }
    }
}
