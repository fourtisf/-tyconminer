using System;
using UnityEngine;
using GoldMineTycoon.Gameplay;

namespace GoldMineTycoon.Core
{
    /// <summary>
    /// Root singleton that owns the save blob, wires up subsystems, drives the
    /// autosave cadence, and resolves offline earnings on resume. One per game.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private float autosaveIntervalSeconds = 30f;

        public SaveData Save { get; private set; }
        public CurrencyManager Currency { get; private set; }
        public ProductionSystem Production { get; private set; }
        public ManagerService Managers { get; private set; }

        private float _autosaveTimer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Save = SaveSystem.Load();
            Currency = new CurrencyManager(Save);
            Production = new ProductionSystem(Save);
            Managers = new ManagerService(Save);
        }

        private void Start()
        {
            ResolveOfflineEarnings();
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            Production.Tick(dt, Managers, Currency);

            _autosaveTimer += dt;
            if (_autosaveTimer >= autosaveIntervalSeconds)
            {
                _autosaveTimer = 0f;
                Persist();
            }
        }

        private void ResolveOfflineEarnings()
        {
            if (Save.savedAtUnix == 0) return;
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var offlineSeconds = Math.Max(0, now - Save.savedAtUnix);
            if (offlineSeconds <= 0) return;

            var earned = IdleCalculator.CalculateOfflineEarnings(
                Save, Production, Managers, offlineSeconds);
            if (earned > 0)
            {
                Currency.Add(CurrencyType.Cash, earned);
                EventBus.Publish(new OfflineEarningsEvent(earned, offlineSeconds));
            }
        }

        public void Persist()
        {
            Production.WriteTo(Save);
            SaveSystem.Save(Save);
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) Persist();
        }

        private void OnApplicationQuit() => Persist();

        private void OnDestroy()
        {
            if (Instance == this) EventBus.Clear();
        }
    }

    public readonly struct OfflineEarningsEvent
    {
        public readonly double Amount;
        public readonly long Seconds;
        public OfflineEarningsEvent(double amount, long seconds)
        {
            Amount = amount;
            Seconds = seconds;
        }
    }
}
