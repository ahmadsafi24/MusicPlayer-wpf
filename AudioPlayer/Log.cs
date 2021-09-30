using System;

namespace AudioPlayer
{
    internal static class Log
    {
        internal static void WriteLine(object message)
        {
           Console.WriteLine($"Log: {message} "); //@{DateTime.Now.TimeOfDay}
        }
    }
}
