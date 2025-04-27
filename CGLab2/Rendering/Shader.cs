using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class Shader : IDisposable
{
    public Shader(string vertexSource, string fragmentSource)
    {
        if (string.IsNullOrWhiteSpace(vertexSource)) throw new ArgumentNullException(nameof(vertexSource));
        if (string.IsNullOrWhiteSpace(fragmentSource)) throw new ArgumentNullException(nameof(fragmentSource));

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);

        GL.CompileShader(vertexShader);

        string vertexInfoLog = GL.GetShaderInfoLog(vertexShader);
        if (vertexInfoLog != string.Empty)
        {
            throw new Exception(vertexInfoLog);
        }


        GL.CompileShader(fragmentShader);

        string fragmentInfoLog = GL.GetShaderInfoLog(fragmentShader);
        if (fragmentInfoLog != string.Empty)
        {
            throw new Exception(fragmentInfoLog);
        }

        ShaderID = GL.CreateProgram();

        GL.AttachShader(ShaderID, vertexShader);
        GL.AttachShader(ShaderID, fragmentShader);

        GL.LinkProgram(ShaderID);
        string linkInfoLog = GL.GetProgramInfoLog(ShaderID);
        if (linkInfoLog != string.Empty)
        {
            throw new Exception(linkInfoLog); 
        }

        GL.DetachShader(ShaderID, vertexShader);
        GL.DetachShader(ShaderID, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public int ShaderID { get; private set; } = 0;

    public void Bind()
    {
        GL.UseProgram(ShaderID);
    }

    #region Uniforms
    public void SetFloat(string name, float x)
    {
        GL.Uniform1(GetUniformLocation(name), x);
    }

    public void SetUInt(string name, uint x)
    {
        GL.Uniform1(GetUniformLocation(name), x);
    }

    public void SetInt(string name, int x)
    {
        GL.Uniform1(GetUniformLocation(name), x);
    }

    public void SetFloat2(string name, float x, float y)
    {
        GL.Uniform2(GetUniformLocation(name), x, y);
    }

    public void SetVector2(string name, Vector2 value)
    {
        GL.Uniform2(GetUniformLocation(name), value);
    }

    public void SetFloat3(string name, float x, float y, float z)
    {
        GL.Uniform3(GetUniformLocation(name), x, y, z);
    }

    public void SetVector3(string name, Vector3 value)
    {
        GL.Uniform3(GetUniformLocation(name), value);
    }

    public void SetFloat4(string name, float x, float y, float z, float w)
    {
        GL.Uniform4(GetUniformLocation(name), x, y, z, w);
    }

    public void SetVector4(string name, Vector4 value)
    {
        GL.Uniform4(GetUniformLocation(name), value);
    }

    public void SetMatrix(string name, ref Matrix4 matrix)
    {
        GL.UniformMatrix4(GetUniformLocation(name), false, ref matrix);
    }

    public void SetColor(string name, ref Color4 color)
    {
        GL.Uniform4(GetUniformLocation(name), color);
    }

    private int GetUniformLocation(string name)
    {
        return GL.GetUniformLocation(ShaderID, name);
    }
    #endregion

    public void Dispose()
    {
        GL.DeleteProgram(ShaderID);
    }
}