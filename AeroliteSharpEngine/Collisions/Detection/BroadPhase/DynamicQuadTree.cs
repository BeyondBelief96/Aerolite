using AeroliteSharpEngine.AeroMath;
using AeroliteSharpEngine.Collisions.Detection.BoundingAreas;
using AeroliteSharpEngine.Core.Interfaces;
using AeroliteSharpEngine.Collisions.Detection.Interfaces;

namespace AeroliteSharpEngine.Collisions.Detection.BroadPhase;

/// <summary>
/// A broadphase spatial partitioning scheme that splits space into quads recursively. Allows configurable amount
/// of objects before partitioning and configurable tree depth. 
/// </summary>
public class DynamicQuadTree : IBroadPhase
{
    // Public struct for visualization data
    public readonly struct QuadTreeNodeData(AeroVec2 center, AeroVec2 halfDimension, int objectCount, bool isLeaf)
    {
        public AeroVec2 Center { get; init; } = center;
        public AeroVec2 HalfDimension { get; init; } = halfDimension;
        
        public bool IsLeaf { get; init; } = isLeaf;
    }
    
    /// <summary>
    /// Represents the node of a quadtree.
    /// </summary>
    /// <param name="center">The center coordinate of this node.</param>
    /// <param name="halfDimension">Vector containing the half width and half height.</param>
    private sealed class QuadTreeNode(AeroVec2 center, AeroVec2 halfDimension)
         {
             public AeroVec2 Center { get; } = center;
             public AeroVec2 HalfDimension { get; } = halfDimension;
             public List<IPhysicsObject2D> Objects { get; } = new();
             public QuadTreeNode?[] Children { get; } = new QuadTreeNode?[4]; // NW, NE, SW, SE
             public bool IsLeaf => Children[0] == null;
     
             public QuadTreeNodeData ToNodeData()
             {
                 return new QuadTreeNodeData(Center, HalfDimension, Objects.Count, IsLeaf);
             }
         }

    private readonly int _maxObjectsPerNode;
    private readonly int _maxDepth;
    private readonly BoundingAreaType _boundingAreaType;
    private readonly QuadTreeNode _root;
    private readonly HashSet<(int, int)> _checkedPairs;

    /// <summary>
    /// Creates a quadtree that covers a rectangular region
    /// </summary>
    /// <param name="width">Width of the region (e.g. screen width)</param>
    /// <param name="height">Height of the region (e.g. screen height)</param>
    /// <param name="origin">Origin point of the region (e.g. (0,0) for top-left)</param>
    /// <param name="boundingAreaType">Type of bounding area to use for broad phase</param>
    /// <param name="maxObjectsPerNode">Maximum objects per node before splitting</param>
    /// <param name="maxDepth">Maximum depth of the tree</param>
    public DynamicQuadTree(
        float width,
        float height,
        AeroVec2 origin,
        BoundingAreaType boundingAreaType,
        int maxObjectsPerNode = 8,
        int maxDepth = 6)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(width, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(height, 0);
        
        _maxObjectsPerNode = maxObjectsPerNode;
        _maxDepth = maxDepth;
        _boundingAreaType = boundingAreaType;
        _checkedPairs = [];
        BoundingAreaType = boundingAreaType;
        MaxObjectsPerNode = maxObjectsPerNode;
        MaxDepth = maxDepth;

        // Calculate the center point of the region
        var center = new AeroVec2(
            origin.X + width * 0.5f,
            origin.Y + height * 0.5f
        );
        
        // Half dimensions represent half width and half height
        var halfDimensions = new AeroVec2(width * 0.5f, height * 0.5f);
        
        _root = new QuadTreeNode(center, halfDimensions);
    }
    
    public BoundingAreaType BoundingAreaType { get; }
    public int MaxObjectsPerNode { get; }
    public int MaxDepth { get; }


    public void Update(IEnumerable<IPhysicsObject2D?> objects)
    {
        ArgumentNullException.ThrowIfNull(objects);
        
        // Clear existing tree but reuse root
        ClearNode(_root);
        _checkedPairs.Clear();

        // Insert all objects
        foreach (var obj in objects)
        {
            if (obj != null) 
            {
                Insert(_root, obj, 0);
            }
        }
    }

    public IEnumerable<(IPhysicsObject2D, IPhysicsObject2D)> FindPotentialCollisions()
    {
        var potentialCollisions = new List<(IPhysicsObject2D, IPhysicsObject2D)>();
        CheckNodeCollisions(_root, potentialCollisions);
        return potentialCollisions;
    }

    /// <summary>
    /// Gets all nodes in the quadtree for visualization purposes
    /// </summary>
    public IEnumerable<QuadTreeNodeData> GetAllNodes()
    {
        var nodes = new List<QuadTreeNodeData>();
        GatherNodes(_root, nodes);
        return nodes;
    }

    private static void GatherNodes(QuadTreeNode? node, List<QuadTreeNodeData> nodes)
    {
        if (node == null) return;

        nodes.Add(node.ToNodeData());

        if (!node.IsLeaf)
        {
            foreach (var child in node.Children)
            {
                GatherNodes(child, nodes);
            }
        }
    }

    private void Insert(QuadTreeNode? node, IPhysicsObject2D obj, int depth)
    {
        if (node == null) return;

        // If this node intersects with the object's bounds
        if (!ObjectIntersectsNode(obj, node))
            return;

        // If we're at a leaf node with capacity or max depth
        if (node.IsLeaf && (node.Objects.Count < _maxObjectsPerNode || depth >= _maxDepth))
        {
            node.Objects.Add(obj);
            return;
        }

        // If we're at a leaf node, split it
        if (node.IsLeaf)
        {
            SplitNode(node);
            
            // Redistribute existing objects
            var existingObjects = new List<IPhysicsObject2D>(node.Objects);
            node.Objects.Clear();
            
            foreach (var existingObj in existingObjects)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (node.Children[i] != null)
                    {
                        Insert(node.Children[i], existingObj, depth + 1);
                    }
                }
            }
        }

        // Insert into children
        for (int i = 0; i < 4; i++)
        {
            if (node.Children[i] != null)
            {
                Insert(node.Children[i], obj, depth + 1);
            }
        }
    }

    private static void SplitNode(QuadTreeNode node)
    {
        var quarterDim = node.HalfDimension * 0.5f;
        
        // Create four children nodes
        node.Children[0] = new QuadTreeNode( // NW
            new AeroVec2(node.Center.X - quarterDim.X, node.Center.Y + quarterDim.Y), 
            quarterDim);
        node.Children[1] = new QuadTreeNode( // NE
            new AeroVec2(node.Center.X + quarterDim.X, node.Center.Y + quarterDim.Y),
            quarterDim);
        node.Children[2] = new QuadTreeNode( // SW
            new AeroVec2(node.Center.X - quarterDim.X, node.Center.Y - quarterDim.Y),
            quarterDim);
        node.Children[3] = new QuadTreeNode( // SE
            new AeroVec2(node.Center.X + quarterDim.X, node.Center.Y - quarterDim.Y),
            quarterDim);
    }

    private void CheckNodeCollisions(QuadTreeNode? node, List<(IPhysicsObject2D, IPhysicsObject2D)> potentialCollisions)
    {
        if (node == null) return;

        // Check collisions between objects in this node
        for (int i = 0; i < node.Objects.Count; i++)
        {
            for (int j = i + 1; j < node.Objects.Count; j++)
            {
                var pair = MakePairId(node.Objects[i].Id, node.Objects[j].Id);
                if (_checkedPairs.Add(pair))
                {
                    potentialCollisions.Add((node.Objects[i], node.Objects[j]));
                }
            }
        }

        // If not a leaf, recurse into children
        if (!node.IsLeaf)
        {
            for (int i = 0; i < 4; i++)
            {
                if (node.Children[i] != null)
                {
                    CheckNodeCollisions(node.Children[i], potentialCollisions);
                }
            }
        }
    }

    private bool ObjectIntersectsNode(IPhysicsObject2D obj, QuadTreeNode node)
    {
        // Create AABB for the node
        var nodeAABB = new AABB2D(node.Center, node.HalfDimension);
        
        // Get object's bounding area
        IBoundingArea objBound = _boundingAreaType switch
        {
            BoundingAreaType.AABB => AABB2D.CreateFromShape(obj.Shape, obj.Position),
            BoundingAreaType.BoundingCircle => BoundingCircle.CreateFromShape(obj.Shape, obj.Position),
            _ => throw new ArgumentException("Unsupported bounding area type")
        };

        return nodeAABB.Intersects(objBound);
    }

    private static void ClearNode(QuadTreeNode node)
    {
        node.Objects.Clear();
        for (int i = 0; i < 4; i++)
        {
            node.Children[i] = null;
        }
    }

    private static (int, int) MakePairId(int id1, int id2)
    {
        return id1 < id2 ? (id1, id2) : (id2, id1);
    }
}