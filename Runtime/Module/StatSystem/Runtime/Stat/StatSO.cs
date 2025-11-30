using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_EDITOR
using System.Text.RegularExpressions;
#endif

namespace NIX.Module.StatSystem
{
    [System.Serializable]
    public class StatLevel
    {
        public float BaseValue;
        public float MaxFinalValue;
    }

    [CreateAssetMenu(menuName = "Stat System/Stat", fileName = "[Name][StatId]")]
    public class StatSO : ScriptableObject
    {
        public string StatId;
        public StatLevel[] Levels;

        protected readonly Dictionary<BonusType, LinkedList<BonusSO>> _Bonuses = new();

        protected float _FinalValue;
        protected int _LevelIndex = 0;

        public event Action<StatSO> onChanged;

        public virtual float FinalValue => _FinalValue;

        protected void OnEnable()
        {
            _LevelIndex = 0;
            _FinalValue = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StatLevel GetStat(int levelIndex)
        {
            levelIndex = Mathf.Clamp(levelIndex, 0, Levels.Length - 1);
            return Levels[levelIndex];
        }
        
        public LinkedList<BonusSO> GetBonusesByType(BonusType type)
        {
            if (!_Bonuses.TryGetValue(type, out LinkedList<BonusSO> bonuses))
            {
                _Bonuses[type] = bonuses = new LinkedList<BonusSO>();
            }
            return _Bonuses[type];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLevel(int level)
        {
            level = Mathf.Clamp(level, 1, Levels.Length);
            _LevelIndex = level - 1;
            _FinalValue = CalculateFinalValue();
            onChanged?.Invoke(this);
        }

        public virtual void UpdateStat(float dt)
        {
            bool recalculateFinalValue = false;
            foreach (KeyValuePair<BonusType, LinkedList<BonusSO>> bonusType in _Bonuses)
            {
                LinkedList<BonusSO> listBonus = bonusType.Value;
                LinkedListNode<BonusSO> iterator = listBonus.First;
                while (iterator != null)
                {
                    BonusSO bns = iterator.Value;
                    bns.CalcTime(dt);
                    if (bns.IsEnded)
                    {
                        listBonus.Remove(iterator);
                        recalculateFinalValue = true;
                    }

                    iterator = iterator.Next;
                }
            }

            if (!recalculateFinalValue) return;
            _FinalValue = CalculateFinalValue();
            onChanged?.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual Dictionary<BonusType, LinkedList<BonusSO>> GetBonuses()
        {
            return _Bonuses;
        }
        
        public virtual LinkedList<BonusSO> GetBonusesByTag(string bonusTag)
        {
            LinkedList<BonusSO> result = new LinkedList<BonusSO>();
            foreach (KeyValuePair<BonusType, LinkedList<BonusSO>> bonusType in _Bonuses)
            {
                LinkedList<BonusSO> listBonus = bonusType.Value;
                foreach (var bns in listBonus)
                {
                    if (bns.BonusTag == bonusTag)
                    {
                        result.AddLast(bns);
                    }
                }
            }

            return result;
        }
        
        public virtual bool HasBonusTag(string bonusTag)
        {
            foreach (KeyValuePair<BonusType, LinkedList<BonusSO>> bonusType in _Bonuses)
            {
                LinkedList<BonusSO> listBonus = bonusType.Value;
                foreach (var bns in listBonus)
                {
                    if (bns.BonusTag == bonusTag)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public float GetBaseValue()
        {
            float baseValue = GetStat(_LevelIndex).BaseValue;
            var bonuses = GetBonusesByType(BonusType.BaseValue);

            baseValue += bonuses.Sum(b => b.Value);

            return baseValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void AddBonus(BonusSO bns)
        {
            if (bns == null) return;
            bns.ResetBonus();
            GetBonusesByType(bns.Type).AddLast(bns);
            _FinalValue = CalculateFinalValue();
            onChanged?.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void RemoveBonus(BonusSO bns)
        {
            if (bns == null) return;
            GetBonusesByType(bns.Type).Remove(bns);
            _FinalValue = CalculateFinalValue();
            onChanged?.Invoke(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void RemoveAllBonus()
        {
            foreach (LinkedList<BonusSO> bonuses in _Bonuses.Values)
            {
                bonuses.Clear();
            }

            CalculateFinalValue();
        }

        public virtual void RemoveBonusByTag(string bonusTag)
        {
            bool recalculateFinalValue = false;

            foreach (KeyValuePair<BonusType, LinkedList<BonusSO>> bonusType in _Bonuses)
            {
                LinkedList<BonusSO> listBonus = bonusType.Value;
                LinkedListNode<BonusSO> iterator = listBonus.First;

                while (iterator != null)
                {
                    var current = iterator;
                    iterator = iterator.Next;

                    if (current.Value.BonusTag != bonusTag) continue;
                    listBonus.Remove(current);
                    recalculateFinalValue = true;
                }
            }

            if (!recalculateFinalValue) return;
            _FinalValue = CalculateFinalValue();
            onChanged?.Invoke(this);
        }

        protected virtual float CalculateFinalValue()
        {
            float newFinalValue = 0;
            if (GetBonusesByType(BonusType.Absolute).Count > 0)
            {
                newFinalValue = GetBonusesByType(BonusType.Absolute).Last.Value.Value;
            }
            else
            {
                float baseValue = GetStat(_LevelIndex).BaseValue;

                LinkedList<BonusSO> bnsBase = GetBonusesByType(BonusType.BaseValue);
                LinkedList<BonusSO> bnsPercent = GetBonusesByType(BonusType.Percent);
                LinkedList<BonusSO> bnsNum = GetBonusesByType(BonusType.Number);

                baseValue += bnsBase.Sum(bns => bns.Value);
                float percentValue = bnsPercent.Sum(bns => bns.Value);
                float numberValue = bnsNum.Sum(bns => bns.Value);
                newFinalValue = baseValue + (baseValue * (percentValue / 100f)) + numberValue;
            }

            return Math.Min(newFinalValue, this.GetStat(_LevelIndex).MaxFinalValue);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            string[] fields = name.Split('-');
            if (fields.Length != 2) return;
            this.StatId = fields[1];
        }
#endif
    }
}