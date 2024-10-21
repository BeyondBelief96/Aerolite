using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Helpers;
using AeroliteSharpEngine.Interfaces;

namespace AeroliteSharpEngine
{
    public class AeroWorld2D
    {
        private List<AeroParticle2D> _particles;
        private List<AeroVec2> _globalForces;
        private ForceRegistry _forceRegistry;
        private float _gravity;

        public AeroWorld2D(float gravity = 9.8f)
        {
            _gravity = gravity;
            _particles = new List<AeroParticle2D>();
            _globalForces = new List<AeroVec2>();
            _forceRegistry = new ForceRegistry();
        }

        public IReadOnlyList<AeroParticle2D> GetParticles() => _particles;

        public void ClearWorld()
        {
            _particles.Clear();
            _globalForces.Clear();
            _forceRegistry.Clear();
        }

        public void AddParticle(AeroParticle2D particle)
        {
            _particles.Add(particle);
        }

        public void AddForceGenerator(IPhysicsObject particle, IForceGenerator generator)
        {
            _forceRegistry.Add(particle, generator);
        }

        public void AddGlobalForce(AeroVec2 force)
        {
            _globalForces.Add(force);
        }

        public void Update(float dt)
        {
            // Update force generators
            _forceRegistry.UpdateForces(dt);

            // Apply global forces and integrate particles
            foreach (var particle in _particles)
            {
                if (particle.IsStatic) continue;

                // Apply gravity if enabled
                if (_gravity != 0)
                {
                    var weight = new AeroVec2(0.0f, particle.Mass * _gravity * AeroConstants.PIXELS_PER_METER);
                    particle.ApplyForce(weight);
                }

                // Apply global forces
                foreach (var force in _globalForces)
                {
                    particle.ApplyForce(force);
                }

                particle.Integrate(dt);
            }
        }
    }
}