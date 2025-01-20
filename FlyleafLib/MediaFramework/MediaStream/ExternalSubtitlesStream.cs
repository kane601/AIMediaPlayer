﻿namespace FlyleafLib.MediaFramework.MediaStream;

public class ExternalSubtitlesStream : ExternalStream
{
    public bool     Downloaded      { get; set; }
    public Language Language        { get; set; } = Language.Unknown;
    public float    Rating          { get; set; } // 1.0-10.0 (0: not set)
    // TODO: Add confidence rating (maybe result is for other movie/episode) | Add Weight calculated based on rating/downloaded/confidence (and lang?) which can be used from suggesters
    public string   Title           { get; set; }
}
