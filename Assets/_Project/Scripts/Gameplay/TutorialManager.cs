using System;
using System.Collections.Generic;
using UnityEngine;
using GoldMineTycoon.Monetization;

namespace GoldMineTycoon.Gameplay
{
    /// <summary>
    /// Drives the onboarding flow (brief: must finish in &lt;3 min, target &gt;90%
    /// completion). Each step is a gate that completes when the player performs the
    /// expected action; progress is logged to analytics so we can find drop-off points.
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        [Serializable]
        public class Step
        {
            public string id;
            [TextArea] public string instruction;
            public GameObject highlightTarget;
        }

        [SerializeField] private List<Step> steps = new();

        public int CurrentIndex { get; private set; }
        public bool IsComplete => CurrentIndex >= steps.Count;

        public event Action<Step> StepShown;
        public event Action TutorialFinished;

        public void Begin()
        {
            CurrentIndex = 0;
            ShowCurrent();
        }

        /// <summary>Called by gameplay when the action for the active step is performed.</summary>
        public void CompleteCurrentStep()
        {
            if (IsComplete) return;

            AnalyticsManager.Instance?.TutorialStep(CurrentIndex, true);
            CurrentIndex++;

            if (IsComplete) TutorialFinished?.Invoke();
            else ShowCurrent();
        }

        private void ShowCurrent()
        {
            var step = steps[CurrentIndex];
            AnalyticsManager.Instance?.TutorialStep(CurrentIndex, false);
            StepShown?.Invoke(step);
        }
    }
}
