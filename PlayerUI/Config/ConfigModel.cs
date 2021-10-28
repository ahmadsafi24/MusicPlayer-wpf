using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerUI.Config
{
    public class ConfigModel
    {
        public string LastFile { get; set; } = string.Empty;
        public bool IsDark { get; set; } = false;
        public double WindowsWidth { get; set; }
        public double WindowsHeight { get; set; }
        public double WindowsLeft { get; set; }
        public double WindowsTop { get; set; }

        public int[] EqBandsGain{get; set; }
    }
}
