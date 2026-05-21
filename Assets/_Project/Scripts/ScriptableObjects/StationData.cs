using UnityEngine;

namespace GoldMineTycoon.Data
{
    public enum StationKind { Mine, Crusher, Smelter, Shipping }

    /// <summary>
    /// Designer-authored config for a single production station. Levels scale output
    /// and capacity along a geometric curve so balancing lives in data, not code.
    /// </summary>
    [CreateAssetMenu(menuName = "GoldMineTycoon/Station", fileName = "Station_")]
    public class StationData : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        public StationKind kind;
        [Tooltip("Mine tambang layer this station belongs to (1-5).")]
        public int layer = 1;

        [Header("Production")]
        [Tooltip("Seconds for one production cycle at level 1.")]
        public float baseCycleSeconds = 3f;
        [Tooltip("Items produced per completed cycle at level 1.")]
        public int baseOutputPerCycle = 1;
        [Tooltip("Max items the station can hold before it stalls.")]
        public int baseCapacity = 50;

        [Header("Economy")]
        public double baseUpgradeCost = 100;
        [Tooltip("Cost multiplier per level. 1.15 = +15% each upgrade.")]
        public float upgradeCostGrowth = 1.15f;
        [Tooltip("Output multiplier per level.")]
        public float outputGrowth = 1.10f;
        [Tooltip("Cash earned per shipped item (Shipping stations only).")]
        public double cashPerItem = 1;

        public double UpgradeCost(int currentLevel) =>
            baseUpgradeCost * Mathf.Pow(upgradeCostGrowth, Mathf.Max(0, currentLevel - 1));

        public int OutputAtLevel(int level) =>
            Mathf.Max(1, Mathf.RoundToInt(baseOutputPerCycle * Mathf.Pow(outputGrowth, Mathf.Max(0, level - 1))));

        public int CapacityAtLevel(int level) =>
            baseCapacity + (level - 1) * baseCapacity / 2;
    }
}
