using System;
using System.Collections.Generic;

namespace GoldMineTycoon.Core
{
    /// <summary>
    /// Lightweight typed publish/subscribe hub. Decouples gameplay systems from UI
    /// so production code never holds direct references to UI controllers.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> Handlers = new();

        public static void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            Handlers[type] = Handlers.TryGetValue(type, out var existing)
                ? Delegate.Combine(existing, handler)
                : handler;
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!Handlers.TryGetValue(type, out var existing)) return;

            var remaining = Delegate.Remove(existing, handler);
            if (remaining == null) Handlers.Remove(type);
            else Handlers[type] = remaining;
        }

        public static void Publish<T>(T evt)
        {
            if (Handlers.TryGetValue(typeof(T), out var existing))
                (existing as Action<T>)?.Invoke(evt);
        }

        public static void Clear() => Handlers.Clear();
    }

    // --- Event payloads ---
    public readonly struct CurrencyChangedEvent
    {
        public readonly CurrencyType Type;
        public readonly double NewAmount;
        public CurrencyChangedEvent(CurrencyType type, double newAmount)
        {
            Type = type;
            NewAmount = newAmount;
        }
    }

    public readonly struct ProductionCompletedEvent
    {
        public readonly string StationId;
        public readonly int Amount;
        public ProductionCompletedEvent(string stationId, int amount)
        {
            StationId = stationId;
            Amount = amount;
        }
    }

    public readonly struct LayerUnlockedEvent
    {
        public readonly int LayerId;
        public LayerUnlockedEvent(int layerId) => LayerId = layerId;
    }

    public readonly struct ManagerHiredEvent
    {
        public readonly string ManagerId;
        public ManagerHiredEvent(string managerId) => ManagerId = managerId;
    }
}
