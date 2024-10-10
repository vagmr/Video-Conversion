namespace VideoConverter
{
    public static class Constants
    {
        public static readonly string[] ValidPresets = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow" };
        public static readonly string[] ValidAudioCodecs = { "aac", "copy", "mp3" };
        public const int MinCrf = 0;
        public const int MaxCrf = 51;
    }
}
