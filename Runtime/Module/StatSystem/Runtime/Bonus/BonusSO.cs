using System;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using System.Text.RegularExpressions;
#endif

namespace NIX.Module.StatSystem
{
    public enum BonusType
    {
        Percent,
        Number,
        Absolute,
        BaseValue
    }

    [CreateAssetMenu(menuName = "Stat System/Bonus", fileName = "[Bonus Name][StatId]")]
    public class BonusSO : ScriptableObject
    {
        public string StatId;
        public string BonusTag;
        public float Value;
        public BonusType Type;
        public float Duration;

        public float RemainingTime { get; protected set; }

        public bool IsEnded => (RemainingTime <= 0) && Duration != 0f;

        public void CalcTime(float dt)
        {
            RemainingTime -= dt;
        }

        public void ResetBonus()
        {
            RemainingTime = Duration;
        }

        public override string ToString()
        {
            return $"StatID: {StatId}, Type: {Type}, Tag: {BonusTag}, Value: {Value}";
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            Regex multiBracketRegex = new Regex(@"^(?:\[[^\]]+\])+$", RegexOptions.Compiled);
            if (!multiBracketRegex.IsMatch(this.name)) return;
            var matches = Regex.Matches(this.name, @"\[(.*?)\]");

            string[] result = matches
                .Select(m => m.Groups[1].Value)
                .ToArray();
            this.StatId = result.Last();
        }
#endif
    }
}