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

    private SubMeshInfo[] _subMeshes;
    public IReadOnlyList<SubMeshInfo> SubMeshes => _subMeshes;

    private int _indicesCount;

    public int VAO => _vao.Pointer;
    public int IndicesCount => _indicesCount;

    public Mesh(Vertex[] vertices, uint[] indices, SubMeshInfo[] subMeshes)
    {
        BuildMesh(vertices, indices);

        _indicesCount = indices.Length;
        _subMeshes = subMeshes;
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
        _vao.LinkAttribute(_vbo, 0, 3, VertexAttribPointerType.Float, sizeof(float) * 5, 0);
        _vao.LinkAttribute(_vbo, 1, 2, VertexAttribPointerType.Float, sizeof(float) * 5, sizeof(float) * 3);

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