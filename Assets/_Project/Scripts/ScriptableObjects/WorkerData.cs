using UnityEngine;

namespace GoldMineTycoon.Data
{
    /// <summary>
    /// Visual/behaviour config for a worker prefab assigned to a station. Workers are
    /// cosmetic in the production math; their animations sell the "factory is alive" feel.
    /// </summary>
    [CreateAssetMenu(menuName = "GoldMineTycoon/Worker", fileName = "Worker_")]
    public class WorkerData : ScriptableObject
    {
        public string id;
        public string displayName;
        public GameObject prefab;
        [Tooltip("Walk speed in world units/second.")]
        public float moveSpeed = 2f;
    }
}
