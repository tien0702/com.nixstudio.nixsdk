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
        public float StartTime;
        public float EndTime;

        public float Duration => EndTime - StartTime;

        public AudioMarker()
        {
            Type = MarkerType.Cue;
        }

        public AudioMarker(string name, float startTime)
        {
            Name = name;
            StartTime = startTime;
            Type = MarkerType.Cue;
            EndTime = 0;
        }
    }
}