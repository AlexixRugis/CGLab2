using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using Dear_ImGui_Sample;
using ImGuiNET;
using System.Net.WebSockets;

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
    private Renderer _renderer = new Renderer();

    private readonly Vertex[] _vertices =
    {
        new Vertex() { Position = new Vector3(0.5f, 0.5f, 0.0f), UV = new Vector2(1.0f,0.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f, 0.0f), UV = new Vector2(1.0f,1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, 0.0f), UV = new Vector2(0.0f,1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, 0.5f, 0.0f), UV = new Vector2(0.0f,0.0f) }
    };

    private readonly Vertex[] _vertices2 =
    {
        new Vertex() { Position = new Vector3(0.5f, 2.5f, 0.5f), UV = new Vector2(1.0f,0.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f, 0.5f), UV = new Vector2(1.0f,1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, -0.5f), UV = new Vector2(0.0f,1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, 2.5f, -0.5f), UV = new Vector2(0.0f,0.0f) },

        new Vertex() { Position = new Vector3(0.5f, 2.5f, -0.5f), UV = new Vector2(1.0f,0.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f, -0.5f), UV = new Vector2(1.0f,1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, 0.5f), UV = new Vector2(0.0f,1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, 2.5f, 0.5f), UV = new Vector2(0.0f,0.0f) },
    };

    private readonly uint[] _indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    private readonly uint[] _indices2 =
    {
        0, 1, 3,
        1, 2, 3,

        4, 5, 7,
        5, 6, 7
    };

    private Mesh _mesh;
    private Mesh _mesh2;
    private Texture _texture;
    private Texture _texture2;
    private Texture _texture3;
    private Texture _texture4;

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

        string texturepath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Textures/cat.png");
        string texturepath2 = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Textures/seal.jpg");
        string texturepath3 = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Textures/beach.png");
        string texturepath4 = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Textures/palm.png");
        _texture = new Texture(new Bitmap(texturepath), true);
        _texture2 = new Texture(new Bitmap(texturepath2), true);
        _texture3 = new Texture(new Bitmap(texturepath3), true);
        _texture4 = new Texture(new Bitmap(texturepath4), true);
        _mesh = new Mesh(_vertices, _indices);
        _mesh2 = new Mesh(_vertices2, _indices2);

        Entity floor = new Entity("Floor");
        floor.AddComponent(new StaticMeshComponent() { Mesh = _mesh, Texture = _texture3 });
        floor.Transform.Rotation = Quaternion.FromEulerAngles(0.5f * MathF.PI, 0.0f, 0.0f);
        floor.Transform.Position.Y = -0.5f;
        floor.Transform.Scale = Vector3.One * 10.0f;

        Entity meshEntity3 = new Entity("Mesh3");
        meshEntity3.Transform.Position.X -= 2.0f;
        meshEntity3.AddComponent(new StaticMeshComponent() { Mesh = _mesh2, Texture = _texture4 });

        Entity meshEntity = new Entity("Mesh");
        meshEntity.AddComponent(new StaticMeshComponent() { Mesh = _mesh, Texture = _texture2 });
        meshEntity.AddComponent(new RotatorComponent() { Speed = 1.0f });

        Entity meshEntity2 = new Entity("Mesh2");
        meshEntity2.AddComponent(new StaticMeshComponent() { Mesh = _mesh, Texture = _texture });
        meshEntity2.AddComponent(new RotatorComponent() { Speed = 3.0f });
        meshEntity2.Transform.Position.X += 2.0f;

        Entity cameraEntity = new Entity("Camera");
        Camera cam = new Camera();
        cameraEntity.AddComponent(cam);
        cameraEntity.AddComponent(new FreeCameraController());
        cam.FOV = 60;
        cam.NearPlane = 0.1f;
        cam.FarPlane = 100f;
        cam.ClearColor = Color.White;
        cam.IsOrthograthic = false;
        cam.ClearColor = Color.DeepSkyBlue;
        cam.Entity.Transform.Position.Z = 3;
        cam.Entity.Transform.Position.Y = 1;

        _world.AddEntity(meshEntity);
        _world.AddEntity(meshEntity2);
        _world.AddEntity(floor);
        _world.AddEntity(meshEntity3);
        _world.AddEntity(cameraEntity);
        _world.CurrentCamera = cam;

        _renderer.OnLoad();

        _world.OnStart();
    }

    public static void Start()
    {
        Instance.Run();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        _world.DestroyAll();
        _renderer.OnUnload();

        _mesh.Dispose();
        _mesh2.Dispose();
        _texture.Dispose();
        _texture2.Dispose();
        _texture3.Dispose();
        _texture4.Dispose();
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
        if (KeyboardState.IsKeyPressed(Keys.F5))
        {
            _showImGui = !_showImGui;
        }

        foreach (var e in _world.Updatables)
        {
            e.Update((float)args.Time);
        }

        // Rendering
        _renderer.Render(_world);

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

        _world.ProcessDestroy();
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
