using Helper;
using PlayerLibrary.Core;
using PlayerLibrary.Plugin;
using System;
using System.Text;
using static PlayerLibrary.Events;

namespace PlayerLibrary
{
    public class Player
    {
        public readonly PlaybackSession PlaybackSession = new();
        public EqualizerController EqualizerController=new();


    }
}
