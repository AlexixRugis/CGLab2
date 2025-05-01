using OpenTK.Mathematics;
using System.Runtime.InteropServices;

public class BVHMesh
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct BVHNode
    {
        [FieldOffset(0)] public Vector3 Min;
        [FieldOffset(12)] public int IndicesCount;
        [FieldOffset(16)] public Vector3 Max;
        [FieldOffset(28)] public int ChildIndex;
    }

    public int MaxDepth { get; private set; }
    public int PrimitivesPerNode { get; private set; }
    public int MaxPrimitivesPerNode { get; private set; } 
    public Vertex[] Vertices { get; private set; }
    public List<uint> Indices { get; private set; }
    public List<BVHNode> Nodes { get; private set; }

    public BVHMesh(IEnumerable<Vertex> vertices, IEnumerable<uint> indices, int maxDepth = 32, int primitivesPerNode = 32)
    {
        Vertices = vertices.ToArray();
        Indices = new List<uint>();
        Nodes = new List<BVHNode>();
        MaxDepth = maxDepth;
        PrimitivesPerNode = primitivesPerNode;
        MaxPrimitivesPerNode = 0;

        List<uint> indList = new List<uint>(indices);
        int root = AllocNode();
        BuildNode(root, indList, 1);
    }

    private int AllocNode()
    {
        Nodes.Add(new BVHNode());
        return Nodes.Count - 1;
    }

    private void BuildNode(int index, List<uint> indices, int depth)
    {
        if (indices.Count == 0) throw new Exception();

        Vector3 maxPos = Vector3.NegativeInfinity;
        Vector3 minPos = Vector3.PositiveInfinity;

        for (int i = 0; i < indices.Count; i++)
        {
            Vector3 v = Vertices[indices[i]].Position;

            if (v.X > maxPos.X) maxPos.X = v.X;
            if (v.Y > maxPos.Y) maxPos.Y = v.Y;
            if (v.Z > maxPos.Z) maxPos.Z = v.Z;

            if (v.X < minPos.X) minPos.X = v.X;
            if (v.Y < minPos.Y) minPos.Y = v.Y;
            if (v.Z < minPos.Z) minPos.Z = v.Z;
        }

        int primitivesCount = indices.Count / 3;

        if (primitivesCount <= PrimitivesPerNode || depth >= MaxDepth)
        {
            // node is a leaf
            
            int startIndex = Indices.Count;
            Indices.AddRange(indices);

            Nodes[index] = new BVHNode()
            {
                Min = minPos,
                Max = maxPos,
                ChildIndex = startIndex,
                IndicesCount = indices.Count
            };

            MaxPrimitivesPerNode = Math.Max(MaxPrimitivesPerNode, primitivesCount);
        }
        else
        {
            List<uint> left = new List<uint>();
            List<uint> right = new List<uint>();

            Vector3 size = maxPos - minPos;
            // split over longest axis
            if (size.X >= size.Y && size.X >= size.Z)
            {
                float halfX = (minPos.X + maxPos.X) * 0.5f;
                
                for (int i = 0; i < indices.Count; i += 3)
                {
                    Vector3 midPoint = (Vertices[indices[i]].Position
                        + Vertices[indices[i + 1]].Position
                        + Vertices[indices[i + 2]].Position) / 3.0f;
                       
                    if (midPoint.X < halfX)
                    {
                        left.Add(indices[i]);
                        left.Add(indices[i + 1]);
                        left.Add(indices[i + 2]);
                    }
                    else
                    {
                        right.Add(indices[i]);
                        right.Add(indices[i + 1]);
                        right.Add(indices[i + 2]);
                    }
                }
            }
            else if (size.Y >= size.X && size.Y >= size.Z)
            {
                float halfY = (minPos.Y + maxPos.Y) * 0.5f;

                for (int i = 0; i < indices.Count; i += 3)
                {
                    Vector3 midPoint = (Vertices[indices[i]].Position
                        + Vertices[indices[i + 1]].Position
                        + Vertices[indices[i + 2]].Position) / 3.0f;

                    if (midPoint.Y < halfY)
                    {
                        left.Add(indices[i]);
                        left.Add(indices[i + 1]);
                        left.Add(indices[i + 2]);
                    }
                    else
                    {
                        right.Add(indices[i]);
                        right.Add(indices[i + 1]);
                        right.Add(indices[i + 2]);
                    }
                }
            }
            else
            {
                float halfZ = (minPos.Z + maxPos.Z) * 0.5f;

                for (int i = 0; i < indices.Count; i += 3)
                {
                    Vector3 midPoint = (Vertices[indices[i]].Position
                        + Vertices[indices[i + 1]].Position
                        + Vertices[indices[i + 2]].Position) / 3.0f;

                    if (midPoint.Z < halfZ)
                    {
                        left.Add(indices[i]);
                        left.Add(indices[i + 1]);
                        left.Add(indices[i + 2]);
                    }
                    else
                    {
                        right.Add(indices[i]);
                        right.Add(indices[i + 1]);
                        right.Add(indices[i + 2]);
                    }
                }
            }

            if (left.Count == 0 || right.Count == 0)
            {
                // one child is empty... Lets make this node a leaf.

                int startIndex = Indices.Count;
                Indices.AddRange(indices);

                Nodes[index] = new BVHNode()
                {
                    Min = minPos,
                    Max = maxPos,
                    ChildIndex = startIndex,
                    IndicesCount = indices.Count
                };

                MaxPrimitivesPerNode = Math.Max(MaxPrimitivesPerNode, primitivesCount);

                return;
            }

            int leftChild = AllocNode();
            int rightChild = AllocNode();

            Nodes[index] = new BVHNode()
            {
                Min = minPos,
                Max = maxPos,
                ChildIndex = leftChild,
                IndicesCount = 0
            };

            indices.Clear();

            BuildNode(leftChild, left, depth + 1);
            BuildNode(rightChild, right, depth + 1);
        }
    }
}