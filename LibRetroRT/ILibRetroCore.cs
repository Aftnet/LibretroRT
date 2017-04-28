using LibRetroRT.AV;
using System;
using System.Collections.Generic;

namespace LibRetroRT
{
    public delegate IReadOnlyList<byte> ReadDataDelegate(ulong count, ulong offset);

    public delegate void RenderVideoFrameDelegate(IReadOnlyList<byte> data, uint width, uint height, ulong pitch);

    public delegate void RenderAudioFramesDelegate(IReadOnlyList<AudioFrame> frames);

    public interface ILibRetroCore : IDisposable
    {
        string Name { get; }
        string Version { get; }
        string SupportedExtensions { get; }
        SystemAVInfo AVInfo { get; }

        event ReadDataDelegate ReadData;

        event RenderVideoFrameDelegate RenderVideoFrame;

        event RenderAudioFramesDelegate RenderAudioFrames;

        void StartExecution(ulong dataSize);
        void EndExecution();
        void Reset();
    }
}
