using System.Collections.Generic;
using UnityEngine;

namespace GoldMineTycoon.Monetization
{
    /// <summary>
    /// Scaffold for Firebase Analytics (brief 9.5). Firebase SDK is NOT yet imported.
    /// Centralizing event logging here means gameplay code calls semantic methods and
    /// never touches the SDK directly, so swapping providers later is a one-file change.
    /// </summary>
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void TutorialStep(int step, bool completed) =>
            Log("tutorial_step", new() { ["step_number"] = step, ["completed"] = completed });

        public void FirstPurchase(string productId, double price, string currency) =>
            Log("first_purchase", new() { ["product_id"] = productId, ["price"] = price, ["currency"] = currency });

        public void IapPurchase(string productId, double price, string currency, double totalRevenue) =>
            Log("iap_purchase", new()
            {
                ["product_id"] = productId, ["price"] = price,
                ["currency"] = currency, ["total_revenue"] = totalRevenue
            });

        public void SubscriptionStart(string productId) =>
            Log("subscription_start", new() { ["product_id"] = productId });

        public void AdShown(string placement, string adType, bool success) =>
            Log("ad_shown", new() { ["placement"] = placement, ["ad_type"] = adType, ["success"] = success });

        public void AdRewardClaimed(string placement, string rewardType) =>
            Log("ad_reward_claimed", new() { ["placement"] = placement, ["reward_type"] = rewardType });

        public void LevelUnlock(int layerId) =>
            Log("level_unlock", new() { ["layer_id"] = layerId });

        public void ManagerHired(string managerId, double cost) =>
            Log("manager_hired", new() { ["manager_id"] = managerId, ["cost"] = cost });

        public void Prestige(int prestigeLevel, double essenceGained) =>
            Log("prestige", new() { ["prestige_level"] = prestigeLevel, ["essence_gained"] = essenceGained });

        public void SessionStart() => Log("session_start", new());
        public void SessionEnd() => Log("session_end", new());

        private void Log(string eventName, Dictionary<string, object> parameters)
        {
            // TODO: FirebaseAnalytics.LogEvent(eventName, parameters-as-Parameter[]).
            Debug.Log($"[Analytics] {eventName} {{{string.Join(", ", parameters)}}}");
        }
    }
}
