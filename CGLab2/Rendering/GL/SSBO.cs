using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

public class SSBO<T> : IDisposable where T : struct
{
    private int _pointer;

    public SSBO(T[] data, bool dynamic = false)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        _pointer = GL.GenBuffer();

        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _pointer);
        GL.BufferData(BufferTarget.ShaderStorageBuffer, Marshal.SizeOf<T>() * data.Length, data, dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
    }

    public int Pointer => _pointer;

    public void Bind(int bindingPoint)
    {
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _pointer);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, bindingPoint, _pointer);
    }

    public void Unbind()
    {
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_pointer);
    }
}