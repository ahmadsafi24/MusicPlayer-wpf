using NAudio.Wave;
using PlayerLibrary.Core;
using System;

namespace PlayerLibrary.Bridge
{
    //Shared between playbacksession and EqualizerController
    public class SampleProviderBridge
    {
        public PlaybackSession PlaybackSession { get; set; }
        public SampleProviderBridge(PlaybackSession playbackSession)
        {
            this.PlaybackSession = playbackSession;
            //EqualizerController = new(this);
        }
        public IWaveProvider InputWaveProvider { get; private set; }

        public IWaveProvider OutputWaveProvider { get; set; }
        internal void Init(IWaveProvider InputWP)
        {
            InputWaveProvider = InputWP;
            EqualizerController.Init(InputWaveProvider.ToSampleProvider());
            //OutputWaveProvider = EqualizerController.OutputWP;
        }
        #region 
        public EqualizerController EqualizerController;


        #endregion
    }
}