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
        /// <param name="physicsObject2D"></param>
        /// <param name="dt"></param>
        void IntegrateLinear(IPhysicsObject2D physicsObject2D, float dt);

        /// <summary>
        /// Performs the integration scheme for angular dynmaic equations.
        /// </summary>
        /// <param name="PhysicsObject"></param>
        /// <param name="dt"></param>
        void IntegrateAngular(IPhysicsObject2D physicsObject2D, float dt);
    }
}
