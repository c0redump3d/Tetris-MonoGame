using System.Collections.Generic;
using System.Linq;

namespace Tetris.Util;

//Thanks to user craftworkgames on StackOverflow for this!
public class FrameCounter
{
    private const int MAXIMUM_SAMPLES = 100;

    private readonly Queue<float> _sampleBuffer = new();
    private long TotalFrames { get; set; }
    private float TotalSeconds { get; set; }
    public float AverageFramesPerSecond { get; private set; }
    public float CurrentFramesPerSecond { get; private set; }

    public void Update(float deltaTime)
    {
        CurrentFramesPerSecond = 1.0f / deltaTime;

        _sampleBuffer.Enqueue(CurrentFramesPerSecond);

        if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
        {
            _sampleBuffer.Dequeue();
            AverageFramesPerSecond = _sampleBuffer.Average(i => i);
        }
        else
        {
            AverageFramesPerSecond = CurrentFramesPerSecond;
        }

        TotalFrames++;
        TotalSeconds += deltaTime;
    }
}