namespace GrowAGarden.Module.Audio
{
    public enum MarkerType
    {
        Cue,
        SubClip
    }

    [System.Serializable]
    public class AudioMarker
    {
        public string Name;
        public string Description;
        public MarkerType Type;
        public float Time;

        public AudioMarker()
        {
            Type = MarkerType.Cue;
        }

        public AudioMarker(string name, float time)
        {
            Name = name;
            Time = time;
            Type = MarkerType.Cue;
        }
    }
}