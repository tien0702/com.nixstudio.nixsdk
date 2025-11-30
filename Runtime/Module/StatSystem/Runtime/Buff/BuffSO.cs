using NIX.Module.StatSystem;
using UnityEngine;

namespace NIX.Module.StatSystem
{
    [CreateAssetMenu(menuName = "Stat System/Buff", fileName = "New Buff")]
    public class BuffSO : ScriptableObject
    {
        public string TargetId;
        public BonusSO[] Bonuses;
    }
}