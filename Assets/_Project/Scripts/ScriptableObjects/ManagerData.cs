using UnityEngine;

namespace GoldMineTycoon.Data
{
    /// <summary>
    /// A hireable manager that automates a station and applies a speed/output buff.
    /// Personality flavour (Pak Budi, Pak Joko, ...) lives here for the Manager screen.
    /// </summary>
    [CreateAssetMenu(menuName = "GoldMineTycoon/Manager", fileName = "Manager_")]
    public class ManagerData : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string skillDescription;
        public Sprite portrait;

        [Tooltip("Station id this manager automates.")]
        public string targetStationId;

        public double hireCost = 500;

        [Tooltip("Cycle-speed multiplier applied while hired. 1.0 = no change, 0.5 = twice as fast.")]
        [Range(0.1f, 1f)] public float cycleSpeedMultiplier = 1f;

        [Tooltip("Output multiplier applied while hired.")]
        public float outputMultiplier = 1f;
    }
}
