using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Events.Base
{
    public delegate Task EventHandlerTimeSpan(TimeSpan Time);
    public delegate Task EventHandlerPlaybackState(Enums.PlaybackState newPlaybackState);
    public delegate Task EventHandlerNull();
}
