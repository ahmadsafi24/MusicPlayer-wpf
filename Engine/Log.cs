using System;

namespace Engine
{
    internal static class Log
    {
        internal static void WriteLine(object message)
        {
            Console.WriteLine($"Log: {message} "); //@{DateTime.Now.TimeOfDay}
        }
    }
}
