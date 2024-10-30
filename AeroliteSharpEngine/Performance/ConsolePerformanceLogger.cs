using System.Diagnostics;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine.Performance;

public class ConsolePerformanceLogger : IPerformanceMonitor
{
    private double _accumulatedTime;
    private int _frameCount;
    private readonly Stopwatch _frameTimer;
    private readonly Stopwatch _stepTimer;
    private double _lastAverageStepTime;
    private int _bodyCount;
    private int _particleCount;
    private readonly Queue<double> _stepTimes;
    private const int SampleSize = 60; // Keep last 60 frames of data

    public ConsolePerformanceLogger()
    {
        _frameTimer = new Stopwatch();
        _stepTimer = new Stopwatch();
        _stepTimes = new Queue<double>();
        _frameTimer.Start();
    }

    public bool IsRunning { get; private set; }
    public double FPS { get; private set; }
    public double AverageStepTime => _lastAverageStepTime;
    public int BodyCount => _bodyCount;
    public int ParticleCount => _particleCount;
    public IReadOnlyCollection<double> StepTimes => _stepTimes;

    public void BeginStep()
    {
        _stepTimer.Restart();
 
    }

    public void EndStep(int bodyCount, int particleCount)
    {
        _stepTimer.Stop();
        double stepTime = _stepTimer.Elapsed.TotalMilliseconds;

        _stepTimes.Enqueue(stepTime);
        if (_stepTimes.Count > SampleSize)
            _stepTimes.Dequeue();

        _bodyCount = bodyCount;
        _particleCount = particleCount;
        _frameCount++;
        _accumulatedTime += stepTime;

        // Update stats every second
        if (_frameTimer.ElapsedMilliseconds >= 1000)
        {
            FPS = _frameCount / (_frameTimer.ElapsedMilliseconds / 1000.0);
            _lastAverageStepTime = _accumulatedTime / _frameCount;

            Console.WriteLine(GetStatsString());

            _frameCount = 0;
            _accumulatedTime = 0;
            _frameTimer.Restart();

        }
    }

    private string GetStatsString()
    {
        return $"FPS: {FPS:F1}\n" +
               $"Step Time: {_lastAverageStepTime:F3}ms\n" +
               $"Bodies: {_bodyCount}\n" +
               $"Particles: {_particleCount}";
    }
}