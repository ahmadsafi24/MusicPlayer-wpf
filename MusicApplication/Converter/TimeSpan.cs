namespace MusicApplication.Converter
{
    public static class TimeSpan
    {
        internal const string stringformat = "mm\\:ss";

        public static string ToString(System.TimeSpan value) => value.ToString(stringformat);
    }
}
