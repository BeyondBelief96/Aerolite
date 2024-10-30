using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.BoundingAreas;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;
using AeroliteSharpEngine.Helpers;
using AeroliteSharpEngine.Shapes;

namespace AeroliteSharpEngine.Collisions.Detection.BroadPhase 
{
    /// <summary>
    /// Spatial partitioning scheme that segments the simulation space into equal size
    /// grid cells. The cells are represented as hash tables with "buckets" of objects stored
    /// in them to represent what objects currently occupy the same cell. Only objects in the same
    /// grid cells are considered as potential collisions.
    /// </summary>
    /// <param name="cellSize"></param>
    /// <param name="boundingAreaType">The type of bounding area being used by the collision detection engine.</param>
    public class UniformGrid(BoundingAreaType boundingAreaType, float cellSize = 100.0f) : IBroadPhase
    {
        private readonly Dictionary<long, List<IPhysicsObject2D>> _grid = new();
        public void Update(IEnumerable<IPhysicsObject2D> objects)
        {
            _grid.Clear();

            foreach (var obj in objects)
            {
                // Get all cells this object's bounding area overlaps
                var overlappingCells = GetOverlappingCells(obj);
            
                foreach (var cellCoords in overlappingCells)
                {
                    var hash = Utils.HashCoords(cellCoords.x, cellCoords.y);
                
                    if (!_grid.TryGetValue(hash, out var cell))
                    {
                        cell = [];
                        _grid[hash] = cell;
                    }
                    cell.Add(obj);
                }
            }
        }

        public IEnumerable<(IPhysicsObject2D, IPhysicsObject2D)> FindPotentialCollisions()
        {
            var checkedPairs = new HashSet<(int, int)>();
            var potentialCollisions = new List<(IPhysicsObject2D, IPhysicsObject2D)>();

            foreach (var cell in _grid.Values)
            {
                for (var i = 0; i < cell.Count; i++)
                {
                    for (var j = i + 1; j < cell.Count; j++)
                    {
                        var objA = cell[i];
                        var objB = cell[j];

                        // Create ID pair to avoid duplicate checks
                        var pairId = MakePairId(objA.Id, objB.Id);
                        if (!checkedPairs.Add(pairId))
                            continue;
                        potentialCollisions.Add((objA, objB));
                    }
                }
            }

            return potentialCollisions;
        }
        
        private IEnumerable<(int x, int y)> GetOverlappingCells(IPhysicsObject2D obj)
        {
            // Get min/max bounds based on bounding area type
            var (min, max) = boundingAreaType switch
            {
                BoundingAreaType.AABB => GetAabbBounds(obj),
                BoundingAreaType.BoundingCircle => GetCircleBounds(obj),
                _ => throw new ArgumentException("Unsupported bounding area type")
            };
    
            // Convert bounds to cell coordinates using floor for proper negative handling
            var minCell = GetCellCoords(min);
            var maxCell = GetCellCoords(max);
            

    
            // Return all cells in the range
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    yield return (x, y);
                }
            }
        }
    
        private static (AeroVec2 min, AeroVec2 max) GetAabbBounds(IPhysicsObject2D obj)
        {
            var aabb = AABB2D.CreateFromShape(obj.Shape, obj.Position);
            return (
                new AeroVec2(
                    aabb.Center.X - aabb.HalfExtents.X,
                    aabb.Center.Y - aabb.HalfExtents.Y
                ),
                new AeroVec2(
                    aabb.Center.X + aabb.HalfExtents.X,
                    aabb.Center.Y + aabb.HalfExtents.Y
                )
            );
        }
    
        private static (AeroVec2 min, AeroVec2 max) GetCircleBounds(IPhysicsObject2D obj)
        {
            var circle = BoundingCircle.CreateFromShape(obj.Shape, obj.Position);
            return (
                new AeroVec2(
                    circle.Center.X - circle.Radius,
                    circle.Center.Y - circle.Radius
                ),
                new AeroVec2(
                    circle.Center.X + circle.Radius,
                    circle.Center.Y + circle.Radius
                )
            );
        }
    
        private (int x, int y) GetCellCoords(AeroVec2 position)
        {
            // Use floor division for correct negative coordinate handling
            return (
                (int)Math.Floor(position.X / cellSize),
                (int)Math.Floor(position.Y / cellSize)
            );
        }
    
        private static (int, int) MakePairId(int id1, int id2)
        {
            return id1 < id2 ? (id1, id2) : (id2, id1);
        }
    }
}