using System;
using System.Diagnostics;

namespace AudioPlayer
{
    public static class Log
    {
        public static void WriteLine(object message)
        {
            Console.WriteLine($"Log: {message} "); //@{DateTime.Now.TimeOfDay}
        }
    }
}
