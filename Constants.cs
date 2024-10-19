namespace VideoConverter
{
    public static class Constants
    {

        //此参数仅适用于libx265
        public static readonly string[] ValidPresetsForNvenc = { "default", "slow", "medium", "fast", "hp", "hq", "bd", "ll", "llhq", "llhp", "lossless", "losslesshp", "p1", "p2", "p3", "p4", "p5", "p6", "p7" };
        //此参数适用于ffmpeg的hevc_nvenc
        public static readonly string[] ValidPresetsForLibx265 = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow" };
        public static readonly string[] ValidAudioCodecs = { "aac", "copy", "mp3" };
        public const int MinCrf = 0;
        public const int MaxCrf = 51;
    }
}
