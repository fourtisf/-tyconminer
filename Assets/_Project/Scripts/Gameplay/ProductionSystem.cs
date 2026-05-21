using System.Collections.Generic;
using UnityEngine;
using GoldMineTycoon.Core;
using GoldMineTycoon.Data;

namespace GoldMineTycoon.Gameplay
{
    /// <summary>
    /// Owns every Station and drives the production chain each frame. The chain flows
    /// Mine -> Crusher -> Smelter -> Shipping; a downstream station only auto-pulls when
    /// it has a manager. Shipping converts items to cash.
    ///
    /// Only manager-run stations advance automatically (idle game core loop). Unmanaged
    /// stations are advanced by the view layer on player tap via TickStation().
    /// </summary>
    public class ProductionSystem
    {
        private readonly List<Station> _stations = new();
        private readonly Dictionary<string, Station> _byId = new();

        public IReadOnlyList<Station> Stations => _stations;

        public ProductionSystem(SaveData save)
        {
            foreach (var data in Resources.LoadAll<StationData>("Stations"))
            {
                var level = Mathf.Max(1, save.GetStationLevel(data.id));
                var stored = 0;
                var idx = save.stationIds.IndexOf(data.id);
                if (idx >= 0) stored = save.stationStored[idx];

                var station = new Station(data, level, stored);
                _stations.Add(station);
                _byId[data.id] = station;
            }
            // Process upstream-to-downstream so produced items can flow within one tick.
            _stations.Sort((a, b) => a.Data.kind.CompareTo(b.Data.kind));
        }

        public Station Get(string id) => _byId.GetValueOrDefault(id);

        public void Tick(float dt, ManagerService managers, CurrencyManager currency)
        {
            foreach (var station in _stations)
            {
                if (!managers.HasManagerForStation(station.Data.id)) continue;
                Advance(station, dt, managers, currency);
            }
            FlowChain(managers, currency);
        }

        /// <summary>Manual production triggered by a player tap on an unmanaged station.</summary>
        public void TickStation(Station station, float dt, ManagerService managers, CurrencyManager currency)
        {
            Advance(station, dt, managers, currency);
            FlowChain(managers, currency);
        }

        private void Advance(Station station, float dt, ManagerService managers, CurrencyManager currency)
        {
            var speed = managers.SpeedMultiplierFor(station.Data.id);
            var output = managers.OutputMultiplierFor(station.Data.id);
            var produced = station.Tick(dt, speed, output);

            if (produced > 0 && station.Data.kind == StationKind.Shipping)
            {
                // Shipping stations sell their throughput immediately rather than store.
                var shipped = station.Withdraw(produced);
                currency.Add(CurrencyType.Cash, shipped * station.Data.cashPerItem);
            }
            if (produced > 0)
                EventBus.Publish(new ProductionCompletedEvent(station.Data.id, produced));
        }

        /// <summary>
        /// Push items down the chain by station kind. Each consumer pulls from the prior
        /// kind's combined stock so a faster upstream station feeds a slower downstream one.
        /// </summary>
        private void FlowChain(ManagerService managers, CurrencyManager currency)
        {
            MoveBetween(StationKind.Mine, StationKind.Crusher);
            MoveBetween(StationKind.Crusher, StationKind.Smelter);
            MoveBetween(StationKind.Smelter, StationKind.Shipping);
        }

        private void MoveBetween(StationKind from, StationKind to)
        {
            foreach (var consumer in _stations)
            {
                if (consumer.Data.kind != to || consumer.IsFull) continue;
                var need = consumer.Capacity - consumer.Stored;

                foreach (var producer in _stations)
                {
                    if (producer.Data.kind != from || need <= 0) continue;
                    var moved = producer.Withdraw(need);
                    consumer.Deposit(moved);
                    need -= moved;
                }
            }
        }

        public bool TryUpgrade(string stationId, CurrencyManager currency)
        {
            var station = Get(stationId);
            if (station == null) return false;
            if (!currency.TrySpend(CurrencyType.Cash, station.UpgradeCost)) return false;
            station.Upgrade();
            return true;
        }

        public void WriteTo(SaveData save)
        {
            foreach (var s in _stations)
                save.SetStationLevel(s.Data.id, s.Level, s.Stored);
        }
    }
}
