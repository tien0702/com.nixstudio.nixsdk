using System.Collections.Generic;
using UnityEngine;

namespace GrowAGarden.Module.Audio
{
    public class AudioChainSO : ScriptableObject
    {
        public List<AudioSO> AudioList;
        public float TimePerIndex;
    }
}