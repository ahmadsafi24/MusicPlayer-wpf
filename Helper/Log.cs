using System.Threading.Tasks;
using System.Windows;

namespace Helper
{
    public static class Log
    {
        public static void WriteLine(object message)
        {
            System.Console.WriteLine($"Log: {message} ");
        }

        public static void WriteLine(string message,object value)
        {
            System.Console.WriteLine($"Log: {message}: < {value} >");
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
