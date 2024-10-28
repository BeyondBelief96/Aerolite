using AeroliteSharpEngine.Collision;
using AeroliteSharpEngine.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroliteSharpEngine.Collisions.BroadPhase
{
    public class SpatialHashBroadPhase : IBroadPhase
    {
        public IEnumerable<(IPhysicsObject, IPhysicsObject)> FindPotentialCollisions(IReadOnlyList<IPhysicsObject> objects)
        {
            return [];
        }

        public void Update(IReadOnlyList<IPhysicsObject> objects)
        {
            return;
        }
    }
}
