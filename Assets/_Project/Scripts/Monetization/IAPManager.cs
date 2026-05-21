using System;
using UnityEngine;

namespace GoldMineTycoon.Monetization
{
    /// <summary>
    /// Scaffold for Google Play Billing via Unity IAP (brief 9.1-9.3). The Unity IAP
    /// package is NOT yet imported, so this compiles standalone and exposes the product
    /// catalog + a purchase entry point. Wire the real IStoreListener implementation
    /// once "In-App Purchasing" is added in Package Manager.
    ///
    /// CRITICAL: receipts must be validated server-side (Firebase Functions) before
    /// granting currency — never trust the client. Do not grant in OnPurchaseComplete
    /// until validation returns success.
    /// </summary>
    public class IAPManager : MonoBehaviour
    {
        // Product IDs mirror brief 9.2 / 9.3 exactly — keep in sync with Play Console.
        public static class Products
        {
            public const string StarterPack = "starter_pack_001";
            public const string GemSmall = "gem_pack_small";
            public const string GemMedium = "gem_pack_medium";
            public const string GemLarge = "gem_pack_large";
            public const string GemXL = "gem_pack_xl";
            public const string GemMega = "gem_pack_mega";
            public const string GemUltra = "gem_pack_ultra";
            public const string LegendaryWorker = "legendary_worker";
            public const string MythicPack = "mythic_pack";
            public const string RemoveAds = "remove_ads";

            public const string GoldPassMonthly = "gold_pass_monthly";
            public const string VipMonthly = "vip_monthly";
        }

        public event Action<string> PurchaseSucceeded;
        public event Action<string, string> PurchaseFailed;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            // TODO: build ConfigurationBuilder with the product ids above and call
            // UnityPurchasing.Initialize(this, builder) once the package is present.
            IsInitialized = true;
            Debug.Log("[IAPManager] Scaffold initialized — wire Unity IAP package to go live.");
        }

        public void Buy(string productId)
        {
            if (!IsInitialized)
            {
                PurchaseFailed?.Invoke(productId, "IAP not initialized");
                return;
            }
            // TODO: storeController.InitiatePurchase(productId);
            Debug.Log($"[IAPManager] Buy requested: {productId} (stub)");
        }

        // Call after server-side receipt validation succeeds.
        public void GrantValidatedPurchase(string productId) => PurchaseSucceeded?.Invoke(productId);
    }
}
