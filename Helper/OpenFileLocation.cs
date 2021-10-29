namespace Helper
{
    public static class OpenFileLocation
    {
        public static void Open(string FilePath)
        {
            string Argument = "/select, \"" + FilePath + "\"";
            _ = System.Diagnostics.Process.Start("explorer.exe", Argument);
        }
    }
}
