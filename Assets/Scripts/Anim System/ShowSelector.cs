using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShowTapeSelector
{
    public string showtapeName;
    public string showtapeDate;
    public ShowtapeSegment[] segments;
}
[System.Serializable]
public class ShowtapeSegment
{
    public SignalQ signalQuality = SignalQ.Complete;
    public AudioQ audioQuality = AudioQ.ReelQuality;
    public VideoQ videoQuality = VideoQ.None;
    public ShowQ showVersion;
    public string segmentNumber;
    public string showName;
    public string showLength;
    public enum ShowQ
    {
        Original,
        Edited,
        Duplicate,
        DuplicateEdited,
        NewSignals,
        DuplicateNewSignals,
    }
    public enum AudioQ
    {
        None,
        Unobtained,
        Incomplete,
        Corrupted,
        LowQuality,
        ReelQuality,
        MasteredAudio
    }
    public enum VideoQ
    {
        None,
        Unobtained,
        Incomplete,
        Corrupted,
        LowQuality,
        ReelQuality,
        MasteredVideo
    }
    public enum SignalQ
    {
        None,
        Unobtained,
        Incomplete,
        Corrupted,
        Complete
    }
}
