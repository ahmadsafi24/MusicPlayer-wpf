using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
