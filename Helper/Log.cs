using System;
using System.Threading.Tasks;
using System.Windows;

namespace Helper
{
    public static class Log
    {
        public static void WriteLine(object message)
        {
             Console.WriteLine($"{DateTime.Now.ToLongTimeString()}| Log: {message}");
        }

        public static void WriteLine(string message,object value)
        {
             Console.WriteLine($"Log: {message}: < {value} >");
        }
        public static async Task ShowMessage(string message)
        {
            await Task.Run(() =>
            {
                MessageBox.Show(message);
            });
        }
    }

}
