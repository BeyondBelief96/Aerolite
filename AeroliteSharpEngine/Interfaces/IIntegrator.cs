using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Interfaces
{
    /// <summary>
    /// Abstraction for an integration method.
    /// </summary>
    public interface IIntegrator
    {
        /// <summary>
        /// Performs the integration scheme for linear dynamic equations.
        /// </summary>
        /// <param name="physicsObject"></param>
        /// <param name="dt"></param>
        void IntegrateLinear(IPhysicsObject physicsObject, float dt);

        /// <summary>
        /// Performs the integration scheme for angular dynmaic equations.
        /// </summary>
        /// <param name="PhysicsObject"></param>
        /// <param name="dt"></param>
        void IntegrateAngular(IPhysicsObject physicsObject, float dt);
    }
}
