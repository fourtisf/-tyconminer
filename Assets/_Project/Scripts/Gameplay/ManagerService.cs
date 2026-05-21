using System.Collections.Generic;
using UnityEngine;
using GoldMineTycoon.Core;
using GoldMineTycoon.Data;

namespace GoldMineTycoon.Gameplay
{
    /// <summary>
    /// Tracks which managers are hired and exposes the per-station buffs they grant.
    /// A station with a hired manager auto-produces; without one it only runs while
    /// the player taps (handled by the view layer).
    /// </summary>
    public class ManagerService
    {
        private readonly SaveData _save;
        private readonly Dictionary<string, ManagerData> _catalog = new();

        public ManagerService(SaveData save)
        {
            _save = save;
            foreach (var m in Resources.LoadAll<ManagerData>("Managers"))
                _catalog[m.id] = m;
        }

        public IEnumerable<ManagerData> Catalog => _catalog.Values;

        public bool IsHired(string managerId) => _save.hiredManagerIds.Contains(managerId);

        public bool HasManagerForStation(string stationId) =>
            TryGetManagerForStation(stationId, out _);

        public bool TryHire(string managerId, CurrencyManager currency)
        {
            if (IsHired(managerId) || !_catalog.TryGetValue(managerId, out var data))
                return false;
            if (!currency.TrySpend(CurrencyType.Cash, data.hireCost))
                return false;

            _save.hiredManagerIds.Add(managerId);
            EventBus.Publish(new ManagerHiredEvent(managerId));
            return true;
        }

        public float SpeedMultiplierFor(string stationId) =>
            TryGetManagerForStation(stationId, out var m) ? m.cycleSpeedMultiplier : 1f;

        public float OutputMultiplierFor(string stationId) =>
            TryGetManagerForStation(stationId, out var m) ? m.outputMultiplier : 1f;

        private bool TryGetManagerForStation(string stationId, out ManagerData manager)
        {
            foreach (var id in _save.hiredManagerIds)
            {
                if (_catalog.TryGetValue(id, out var m) && m.targetStationId == stationId)
                {
                    manager = m;
                    return true;
                }
            }
            manager = null;
            return false;
        }
    }
}
