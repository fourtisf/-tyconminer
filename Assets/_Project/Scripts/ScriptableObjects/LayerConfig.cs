using System.Collections.Generic;
using UnityEngine;

namespace GoldMineTycoon.Data
{
    /// <summary>
    /// One of the five mine layers (Surface, Shallow Cave, Deep Mine, Abyss Lava,
    /// Ancient Vault). Bundles the stations available at that depth and the gate to
    /// unlock it.
    /// </summary>
    [CreateAssetMenu(menuName = "GoldMineTycoon/Layer", fileName = "Layer_")]
    public class LayerConfig : ScriptableObject
    {
        public int layerId = 1;
        public string displayName;

        [Tooltip("Reputation required to unlock this layer.")]
        public double unlockReputation;

        public List<StationData> stations = new();
    }
}
