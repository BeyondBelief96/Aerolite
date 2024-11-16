using AeroliteSharpEngine.Core.Interfaces;

namespace AeroliteSharpEngine.Interfaces
{
    /// <summary>
    /// A force generator can be asked to add a force to one or more
    /// physics objects. These can be used to create forces such as gravity,
    /// drag, friction, spring etc. to apply to an object. These are useful
    /// if you're treating our object as a point like object, and applying forces
    /// to its center of mass.
    /// </summary>
    public interface IForceGenerator
    {
        /// <summary>
        /// Implementations of this interface will calculate their
        /// forces and apply them to the given object.
        /// </summary>
        /// <param name="physicsObject2D"></param>
        /// <param name="dt"></param>
        void UpdateForce(IPhysicsObject2D physicsObject2D, float dt);
    }
}
