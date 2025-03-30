using OpenTK.Mathematics;

public struct Vertex
{
    public const int Size = sizeof(float) * 3 + sizeof(float) * 2;

    public Vector3 Position;
    public Vector2 UV;
}