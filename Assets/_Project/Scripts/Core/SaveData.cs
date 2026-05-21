using System;
using System.Collections.Generic;

namespace GoldMineTycoon.Core
{
    /// <summary>
    /// Serializable snapshot of all persistent game state. Kept as plain fields
    /// (no properties) because Unity's JsonUtility only serializes public fields.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int version = 1;
        public long savedAtUnix;

        // Currencies
        public double cash;
        public double gems;
        public double reputation;
        public double essence;

        // Progression
        public int unlockedLayer = 1;
        public int prestigeLevel;

        // Stations: parallel lists keyed by id (JsonUtility can't serialize dictionaries).
        public List<string> stationIds = new();
        public List<int> stationLevels = new();
        public List<int> stationStored = new();

        // Hired managers by id.
        public List<string> hiredManagerIds = new();

        // Monetization
        public bool adsRemoved;
        public bool goldPassActive;
        public bool vipActive;
        public bool starterPackPurchased;

        public int GetStationLevel(string id)
        {
            var i = stationIds.IndexOf(id);
            return i >= 0 ? stationLevels[i] : 0;
        }

        public void SetStationLevel(string id, int level, int stored)
        {
            var i = stationIds.IndexOf(id);
            if (i < 0)
            {
                stationIds.Add(id);
                stationLevels.Add(level);
                stationStored.Add(stored);
            }
            else
            {
                stationLevels[i] = level;
                stationStored[i] = stored;
            }
        }
    }
}
