using System.Threading.Tasks;
using System.Windows;

namespace PlayerLibrary
{
    public static class Log
    {
        public static void WriteLine(object message)
        {
            System.Console.WriteLine($"Log: {message} "); //@{DateTime.Now.TimeOfDay}
            //await ShowMessage($"Log: {message} ");
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
