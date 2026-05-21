using System;
using UnityEngine;

namespace GoldMineTycoon.Core
{
    /// <summary>
    /// Single point of truth for all currency mutations. UI reads through here and
    /// listens to CurrencyChangedEvent rather than polling the save blob.
    /// </summary>
    public class CurrencyManager
    {
        private readonly SaveData _save;

        public CurrencyManager(SaveData save) => _save = save;

        public double Get(CurrencyType type) => type switch
        {
            CurrencyType.Cash => _save.cash,
            CurrencyType.Gem => _save.gems,
            CurrencyType.Reputation => _save.reputation,
            CurrencyType.Essence => _save.essence,
            _ => 0
        };

        public void Add(CurrencyType type, double amount)
        {
            if (amount < 0) throw new ArgumentException("Use Spend for negative amounts.");
            Set(type, Get(type) + amount);
        }

        public bool TrySpend(CurrencyType type, double amount)
        {
            if (amount < 0 || Get(type) < amount) return false;
            Set(type, Get(type) - amount);
            return true;
        }

        private void Set(CurrencyType type, double value)
        {
            switch (type)
            {
                case CurrencyType.Cash: _save.cash = value; break;
                case CurrencyType.Gem: _save.gems = value; break;
                case CurrencyType.Reputation: _save.reputation = value; break;
                case CurrencyType.Essence: _save.essence = value; break;
            }
            EventBus.Publish(new CurrencyChangedEvent(type, value));
        }
    }
}
