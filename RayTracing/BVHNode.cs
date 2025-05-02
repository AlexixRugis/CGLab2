using OpenTK.Mathematics;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit, Size = 32)]
public struct BVHNode
{
    [FieldOffset(0)] public Vector3 Min;
    [FieldOffset(12)] public int IndicesCount;
    [FieldOffset(16)] public Vector3 Max;
    [FieldOffset(28)] public int ChildIndex;
}