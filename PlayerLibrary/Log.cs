namespace PlayerLibrary
{
    public static class Log
    {
        public static void WriteLine(object message)
        {
            System.Console.WriteLine($"Log: {message} "); //@{DateTime.Now.TimeOfDay}
        }
    }
}
