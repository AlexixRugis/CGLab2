using OpenTK.Graphics.OpenGL;

public sealed class Mesh : IDisposable
{
    public struct SubMeshInfo
    {
        public int Index; public int Size;
    }

    private VAO _vao;
    private VBO _vbo;
    private EBO _ebo;

    private int _indicesCount;

    private SubMeshInfo[] _subMeshes;

    private Vertex[]? _vertices = null;
    private uint[]? _indices = null;

    public IReadOnlyList<Vertex>? Vertices => _vertices;
    public IReadOnlyList<uint>? Indices => _indices;
    public IReadOnlyList<SubMeshInfo> SubMeshes => _subMeshes;

    public int VAO => _vao.Pointer;
    public int IndicesCount => _indicesCount;


    public Mesh(Vertex[] vertices, uint[] indices, SubMeshInfo[] subMeshes, bool keepOnCpu = false)
    {
        BuildMesh(vertices, indices);

        _indicesCount = indices.Length;
        _subMeshes = subMeshes;

        if (keepOnCpu)
        {
            _vertices = vertices;
            _indices = indices;
        }
    }

    public void Bind()
    {
        _vao.Bind();
    }

    public void Unbind()
    {
        _vao.Unbind();
    }

    private void BuildMesh(Vertex[] vertices, uint[] indices)
    {
        _vao = new VAO();
        _vao.Bind();

        _ebo = new EBO(indices);
        _ebo.Bind();

        _vbo = new VBO(vertices);
        _vao.LinkAttribute(_vbo, 0, 3, VertexAttribPointerType.Float, Vertex.Size, 0);
        _vao.LinkAttribute(_vbo, 1, 3, VertexAttribPointerType.Float, Vertex.Size, sizeof(float) * 3);
        _vao.LinkAttribute(_vbo, 2, 2, VertexAttribPointerType.Float, Vertex.Size, sizeof(float) * 6);

        _vao.Unbind();
        _ebo.Unbind();
    }

    public void Dispose()
    {
        _vao.Dispose();
        _ebo.Dispose();
        _vbo.Dispose();
    }
}