using System;
using UnityEngine;

namespace GoldMineTycoon.Monetization
{
    /// <summary>
    /// Scaffold for AdMob (brief 9.4). Google Mobile Ads SDK is NOT yet imported, so
    /// this compiles standalone. Rewarded ads gate boosts/double-offline; interstitials
    /// fire every 3-5 min of session. No banners (bad UX for idle).
    ///
    /// Respect SaveData.adsRemoved: callers should skip interstitials when ads are
    /// removed, but rewarded ads stay available (they grant value the player opted into).
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        public enum Placement { BoostProduction, GemChest, DoubleOffline }

        [SerializeField] private float minSecondsBetweenInterstitials = 180f;
        private float _lastInterstitial = -9999f;

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            // TODO: MobileAds.Initialize(...) and pre-load rewarded + interstitial units.
            IsInitialized = true;
            Debug.Log("[AdsManager] Scaffold initialized — wire Google Mobile Ads SDK to go live.");
        }

        /// <summary>Show a rewarded video; <paramref name="onReward"/> fires only on full view.</summary>
        public void ShowRewarded(Placement placement, Action onReward)
        {
            if (!IsInitialized) { onReward?.Invoke(); return; } // dev fallback grants reward
            // TODO: rewardedAd.Show(reward => onReward()).
            Debug.Log($"[AdsManager] Rewarded requested: {placement} (stub grants reward)");
            onReward?.Invoke();
        }

        public bool TryShowInterstitial(bool adsRemoved)
        {
            if (adsRemoved || !IsInitialized) return false;
            if (Time.realtimeSinceStartup - _lastInterstitial < minSecondsBetweenInterstitials)
                return false;

            _lastInterstitial = Time.realtimeSinceStartup;
            // TODO: interstitialAd.Show();
            Debug.Log("[AdsManager] Interstitial shown (stub)");
            return true;
        }
    }
}
