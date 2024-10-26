namespace AeroliteSharpEngine.Interfaces
{
    /// <summary>
    /// Abstraction for an integration method.
    /// </summary>
    public interface IIntegrator
    {
        /// <summary>
        /// Performs the integration scheme.
        /// </summary>
        /// <param name="physicsObject"></param>
        /// <param name="dt"></param>
        void Integrate(IPhysicsObject physicsObject, float dt);
    }
}
