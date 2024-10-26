namespace AeroliteSharpEngine.Interfaces
{
    /// <summary>
    /// Interface used to track and record engine performance metrics.
    /// </summary>
    public interface IPerformanceMonitor
    {
        double FPS { get; }
        double AverageStepTime { get; }
        int BodyCount { get; }
        int ParticleCount { get; }
        IReadOnlyCollection<double> StepTimes { get; }
        void BeginStep();
        void EndStep(int bodyCount, int particleCount);
        string GetStatsString();
    }
}
