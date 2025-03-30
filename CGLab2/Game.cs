using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using Dear_ImGui_Sample;
using ImGuiNET;

public class Game : GameWindow
{
    private const int _width = 1280;
    private const int _height = 720;
    private const string _title = "Test Window";
    private const VSyncMode _vSyncMode = VSyncMode.On;
    private const int antiAliasingLevel = 16;

    private ImGuiController _controller;
    private bool _showImGui = false;
    private bool _vsync = false;


    private static Game _instance;

    public static Game Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Game();
            }

            return _instance;
        }
    }

    private World _world = new World();

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

    private int _counter = 0;

    private Game() : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            ClientSize = new Vector2i(_width, _height),
            Title = _title,
            Flags = ContextFlags.Default,
            RedBits = 8,
            GreenBits = 8,
            BlueBits = 8,
            AlphaBits = 0,
            DepthBits = 24,
            StencilBits = 8,
        })
    {
        VSync = _vSyncMode;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        _controller = new ImGuiController(ClientSize.X, ClientSize.Y);

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

        Entity entity = new Entity("Mesh");
        entity.Mesh = _mesh;

        _world.AddEntity(entity);
        _world.Camera.FOV = 60;
        _world.Camera.NearPlane = 0.1f;
        _world.Camera.FarPlane = 100f;
        _world.Camera.ClearColor = Color.White;
        _world.Camera.IsOrthograthic = false;
        _world.CameraTransform.Position.Z = 4;
    }

    public static void Start()
    {
        Instance.Run();
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

        // Tell ImGui of the new size
        _controller.WindowResized(ClientSize.X, ClientSize.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _controller.Update(this, (float)args.Time);

        Entity ent = _world.FindEntity("Mesh");
        ent.Transform.Rotation *= Quaternion.FromEulerAngles(0, (float)args.Time, 0);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
        if (KeyboardState.IsKeyPressed(Keys.F5))
        {
            _showImGui = !_showImGui;
        }

        // Rendering

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader.Bind();
        Matrix4 view = _world.CameraTransform.WorldToLocal;
        Matrix4 proj = _world.Camera.GetProjectionMatrix();
        _shader.SetMatrix("_View", ref view);
        _shader.SetMatrix("_Projection", ref proj);
        _texture.Bind(TextureUnit.Texture0);

        for (int i = 0; i < _world.Entities.Count; i++)
        {
            Entity e = _world.Entities[i];
            if (e.Mesh == null) continue;

            Matrix4 model = e.Transform.LocalToWorld;
            _shader.SetMatrix("_Model", ref model);
            _mesh.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        }

        // Enable Docking
        if (_showImGui)
        {
            ImGui.Begin("Controls");
            ImGui.Text($"FPS: {(int)(1.0 / args.Time)}");
            ImGui.Checkbox("VSync", ref _vsync);
            if (_vsync && VSync != VSyncMode.On) VSync = VSyncMode.On;
            else if (!_vsync && VSync != VSyncMode.Off) VSync = VSyncMode.Off;

            if (ImGui.Button("fff"))
            {
                Console.WriteLine("fff");
            }



            ImGui.End();

            _controller.Render();

            ImGuiController.CheckGLError("End of frame");
        }

        SwapBuffers();
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);


        _controller.PressChar((char)e.Unicode);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _controller.MouseScroll(e.Offset);
    }
}
