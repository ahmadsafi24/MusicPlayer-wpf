using System;
using System.Diagnostics;

namespace AudioPlayer
{
    public static class Log
    {
        public static void WriteLine(object message)
        {
            Debug.WriteLine($"Log: {message} "); //@{DateTime.Now.TimeOfDay}
        }
    }
}
