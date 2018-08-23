﻿using FDK;

namespace DTXMania
{
    /// <summary>
    /// The ConfigIniToSongGainControllerBinder allows for SONGVOL and/or other
    /// properties related to the Gain levels applied to song preview and
    /// playback, to be applied conditional based on settings flowing from
    /// ConfigIni. This binder class allows that to take place without either
    /// ConfigIni or SongGainController having awareness of one nother.
    /// See those classes properties, methods, and events for more details. 
    /// </summary>
    internal static class ConfigIniToSongGainControllerBinder
    {
        internal static void Bind(CConfigIni configIni, SongGainController songGainController)
        {
            songGainController.ApplySongVol = configIni.ApplySongVol;

            configIni.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(CConfigIni.ApplySongVol):
                        songGainController.ApplySongVol = configIni.ApplySongVol;
                        break;
                }
            };
        }
    }
}