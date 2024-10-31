using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Interfaces
{
    /// <summary>
    /// Interface used to track and record engine performance metrics.
    /// </summary>
    public interface IPerformanceMonitor
    {
        double Fps { get; }
        double AverageStepTime { get; }
        int BodyCount { get; }
        int ParticleCount { get; }
        IReadOnlyCollection<double> StepTimes { get; }
        void BeginStep();
        void EndStep(IAeroPhysicsWorld world);
    }
}
