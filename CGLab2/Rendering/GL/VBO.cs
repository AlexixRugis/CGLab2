using OpenTK.Graphics.OpenGL;

public class VBO : IDisposable
{
    private int _pointer;

    public VBO(Vertex[] data, bool dynamic = false)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        _pointer = GL.GenBuffer();

        Bind();
        GL.BufferData(BufferTarget.ArrayBuffer, Vertex.Size * data.Length, data, dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
        Unbind();
    }

    public int Pointer => _pointer;

    public void Bind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _pointer);
    }

    public void Unbind()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_pointer);
    }
}