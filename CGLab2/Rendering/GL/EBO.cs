using OpenTK.Graphics.OpenGL;

public class EBO : IDisposable
{
    private int _pointer;

    public EBO(uint[] data, bool dynamic = false)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        _pointer = GL.GenBuffer();

        Bind();
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * data.Length, data, dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
        Unbind();
    }

    public int Pointer => _pointer;

    public void Bind()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _pointer);
    }

    public void Unbind()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_pointer);
    }
}