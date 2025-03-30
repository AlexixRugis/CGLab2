using OpenTK.Graphics.OpenGL;

public class VAO : IDisposable
{
    private int _pointer;

    public VAO()
    {
        _pointer = GL.GenVertexArray();
    }

    public int Pointer => _pointer;

    public void LinkAttribute(VBO vbo, int layout, int size, VertexAttribPointerType type, int stride, int offset)
    {
        if (vbo == null) throw new ArgumentNullException(nameof(vbo));

        vbo.Bind();
        GL.VertexAttribPointer(layout, size, type, false, stride, offset);
        GL.EnableVertexAttribArray(layout);
        vbo.Unbind();
    }

    public void Bind()
    {
        GL.BindVertexArray(_pointer);
    }

    public void Unbind()
    {
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(_pointer);
    }
}