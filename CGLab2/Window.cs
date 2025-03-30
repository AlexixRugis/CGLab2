using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

public class Window : GameWindow
{
    private readonly Vertex[] _vertices =
    {
        new Vertex() { Position = new Vector3(0.5f, 0.5f, 0.0f), UV = new Vector2(1.0f,0.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f, 0.0f), UV = new Vector2(1.0f,1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, 0.0f), UV = new Vector2(0.0f,1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, 0.5f, 0.0f), UV = new Vector2(0.0f,0.0f) }
    };

    private readonly uint[] _indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    private Mesh _mesh;
    private Shader _shader;
    private Texture _texture;

    public Window(int width, int height, string title) 
        : base(GameWindowSettings.Default, 
            new NativeWindowSettings() { ClientSize = (width, height), Title = title })
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        GL.Enable(EnableCap.Texture2D);
        GL.Disable(EnableCap.Blend);
        GL.Disable(EnableCap.Lighting);

        string vpath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Shaders/vert.glsl");
        string fpath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Shaders/frag.glsl");
        string texturepath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Textures/cat.png");
        string vtext = File.ReadAllText(vpath);
        string ftext = File.ReadAllText(fpath);
        _shader = new Shader(vtext, ftext);
        _texture = new Texture(new Bitmap(texturepath), true);
        _mesh = new Mesh(_vertices, _indices);
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        _mesh.Dispose();
        _shader.Dispose();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        // Rendering

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _texture.Bind(TextureUnit.Texture0);
        _shader.Bind();
        _mesh.Bind();
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }
}
