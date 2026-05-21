using System.Collections.Generic;
using UnityEngine;

namespace GoldMineTycoon.UI
{
    /// <summary>
    /// Master UI controller. Owns the bottom-nav screens (Factory, Manager, Orders,
    /// Prestige, Shop) and shows exactly one at a time. Individual screens subscribe to
    /// EventBus for their own data; this class only handles navigation.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public enum Screen { Factory, Manager, Orders, Prestige, Shop }

        [SerializeField] private List<ScreenBinding> screens = new();

        [System.Serializable]
        private struct ScreenBinding
        {
            public Screen screen;
            public GameObject root;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start() => Show(Screen.Factory);

        public void Show(Screen screen)
        {
            foreach (var binding in screens)
                if (binding.root != null)
                    binding.root.SetActive(binding.screen == screen);
        }

        // Hooked to bottom-nav buttons in the inspector.
        public void ShowFactory() => Show(Screen.Factory);
        public void ShowManager() => Show(Screen.Manager);
        public void ShowOrders() => Show(Screen.Orders);
        public void ShowPrestige() => Show(Screen.Prestige);
        public void ShowShop() => Show(Screen.Shop);
    }
}
