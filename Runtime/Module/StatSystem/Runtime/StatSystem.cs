using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NIX.Core;
using UnityEngine;

namespace NIX.Module.StatSystem
{
    public class StatSystem : BaseMono
    {
        [SerializeField] protected List<StatSO> _Stats = new();
        protected int _Level = 1;

        protected virtual void Update()
        {
            foreach (var stat in _Stats)
            {
                stat.UpdateStat(Time.deltaTime);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void SetLevel(int level)
        {
            _Level = level;
            foreach (var stat in _Stats)
            {
                stat.SetLevel(level);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int GetLevel() => _Level;

        public virtual void AddBonus(BonusSO bonus)
        {
            if (bonus == null)
            {
                Debug.LogError("Bonus object is null");
                return;
            }

            StatSO stat = _Stats.Find(s => s.StatId == bonus.StatId);
            if (stat == null)
            {
                Debug.LogError($"Stat with ID {bonus.StatId} not found");
                return;
            }

            stat.AddBonus(bonus);
        }

        public virtual void RemoveBonus(BonusSO bonus)
        {
            if (bonus == null)
            {
                Debug.LogError("Bonus object is null");
                return;
            }

            StatSO stat = GetStat(bonus.StatId);
            if (stat == null) return;

            stat.RemoveBonus(bonus);
        }
        
        public virtual void RemoveBonusesByTag(string bonusTag)
        {
            foreach (var stat in _Stats)
            {
                stat.RemoveBonusByTag(bonusTag);
            }
        }

        public virtual void AddStats(List<StatSO> stat)
        {
            _Stats.AddRange(stat);
        }
        
        public virtual List<StatSO> GetStats() => _Stats;

        public virtual void SetStats(List<StatSO> stat)
        {
            this._Stats = stat;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StatSO GetStat(string statId)
        {
            var stat = _Stats.Find(s => s.StatId == statId);
            if (stat == null)
            {
                Debug.LogError($"Stat with ID {statId} not found");
            }
            return stat;
        }
#if UNITY_EDITOR
        [ContextMenu("Log Bonuses")]
        private void LogBonuses()
        {
            string log = string.Empty;
            foreach (var stat in _Stats)
            {
                string statId = stat.StatId;
                log += "Bonus of StatID: " + statId + "\n";
                var bonuses = stat.GetBonuses();
                foreach (var bns in bonuses)
                {
                    if (bns.Value.Count == 0) continue;
                    log += $"    [{bns.Key}]" + "\n";
                    log += $"    Base: {stat.GetBaseValue()}" + "\n";
                    foreach (var b in bns.Value)
                    {
                        log += $"       Value: {b.Value} - Tag: {b.BonusTag} \n";
                    }
                }

                log += "-------------------------------------\n";
            }

            Debug.Log(log);
        }
#endif
    }
}