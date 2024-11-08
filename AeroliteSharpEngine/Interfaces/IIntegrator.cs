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
        /// <param name="physicsObject2D">The physics object to integrate.</param>
        /// <param name="dt">The delta time since the last time step.</param>
        void IntegrateLinear(IPhysicsObject2D physicsObject2D, float dt);

        /// <summary>
        /// Performs the integration scheme for angular dynamic equations.
        /// </summary>
        /// <param name="physicsObject2D">The physics object to integrate.</param>
        /// <param name="dt">The delta time since the last time step.</param>
        void IntegrateAngular(IPhysicsObject2D physicsObject2D, float dt);
    }
}
