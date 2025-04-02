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
    private const int _antiAliasingLevel = 16;


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

    private ImGuiEditor _editor;

    public AssetLoader Assets { get; } = new AssetLoader();
    public World World { get; } = new World();

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
            NumberOfSamples = _antiAliasingLevel
        })
    {
        VSync = _vSyncMode;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        Assets.LoadShader("ShaderTexUnlit",
            "Resources/Shaders/vert.glsl",
            "Resources/Shaders/frag.glsl");

        Assets.LoadTexture("TexCat", "Resources/Textures/cat.png", true);
        Assets.LoadTexture("TexSeal", "Resources/Textures/seal.jpg", true);
        Assets.LoadTexture("TexBeach", "Resources/Textures/beach.png", true);
        Assets.LoadTexture("TexPalm", "Resources/Textures/palm.png", true);

        Assets.LoadMesh("MeshQuad", _vertices, _indices, new Mesh.SubMeshInfo[1] { new() { Index = 0, Size = 6 } });
        Assets.LoadMesh("MeshPalm", _vertices2, _indices2, new Mesh.SubMeshInfo[1] { new() { Index = 0, Size = 12 } });
        Assets.LoadMesh("MeshCylinder", "Resources/Models/cylinder.fbx");

        Entity cylinder = new Entity("Cylinder");
        cylinder.AddComponent(new StaticMeshComponent()
        {
            Mesh = Assets.GetMesh("MeshCylinder"),
            Material = new UnlitTexturedMaterial(Assets.GetTexture("TexBeach"))
        });
        cylinder.Transform.LocalPosition -= Vector3.UnitZ * 3f;

        Entity floor = new Entity("Floor");
        floor.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshQuad"),
            Material = new UnlitTexturedMaterial(Assets.GetTexture("TexBeach"))
        });
        floor.Transform.LocalRotation = Quaternion.FromEulerAngles(0.5f * MathF.PI, 0.0f, 0.0f);
        floor.Transform.LocalPosition += Vector3.UnitY * -0.5f;
        floor.Transform.LocalScale = new Vector3(15.0f, 10.0f, 1.0f);

        Entity meshEntity3 = new Entity("Mesh3");
        meshEntity3.Transform.LocalPosition -= Vector3.UnitX * 2.0f;
        meshEntity3.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshPalm"),
            Material = new UnlitTexturedMaterial(Assets.GetTexture("TexPalm"))
        });

        Entity meshEntity4 = new Entity("Mesh4");
        meshEntity4.Transform.LocalPosition += Vector3.UnitX * 4.0f;
        meshEntity4.AddComponent(new StaticMeshComponent()
        {
            Mesh = Assets.GetMesh("MeshPalm"),
            Material = new UnlitTexturedMaterial(Assets.GetTexture("TexPalm"))
        });

        Entity meshEntity5 = new Entity("Mesh5");
        meshEntity5.Transform.LocalPosition -= Vector3.UnitX * 1.0f;
        meshEntity5.Transform.LocalPosition -= Vector3.UnitZ * 2.0f;
        meshEntity5.AddComponent(new StaticMeshComponent()
        {
            Mesh = Assets.GetMesh("MeshPalm"),
            Material = new UnlitTexturedMaterial(Assets.GetTexture("TexPalm"))
        });

        Entity meshEntity = new Entity("Mesh");
        meshEntity.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshQuad"),
            Material = new UnlitTexturedMaterial(Assets.GetTexture("TexCat"))
        });
        meshEntity.AddComponent(new RotatorComponent() { Speed = 1.0f });

        Entity meshEntity2 = new Entity("Mesh2");
        meshEntity2.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshQuad"),
            Material = new UnlitTexturedMaterial(Assets.GetTexture("TexSeal"))
        });
        meshEntity2.AddComponent(new RotatorComponent() { Speed = 3.0f });
        meshEntity2.Transform.LocalPosition += Vector3.UnitX * 2.0f;

        meshEntity2.Transform.SetParent(meshEntity.Transform);

        Entity cameraEntity = new Entity("Camera");
        Camera cam = new Camera();
        cameraEntity.AddComponent(cam);
        cameraEntity.AddComponent(new FreeCameraController());
        cam.FOV = 60;
        cam.NearPlane = 0.1f;
        cam.FarPlane = 100f;
        cam.ClearColor = Color.White;
        cam.IsOrthograthic = false;
        cam.ClearColor = Color.SkyBlue;
        cam.Entity.Transform.LocalPosition += Vector3.UnitZ * 3;
        cam.Entity.Transform.LocalPosition += Vector3.UnitY * 1;

        World.AddEntity(cylinder);
        World.AddEntity(meshEntity);
        World.AddEntity(meshEntity2);
        World.AddEntity(floor);
        World.AddEntity(meshEntity3);
        World.AddEntity(meshEntity4);
        World.AddEntity(meshEntity5);
        World.AddEntity(cameraEntity);
        World.CurrentCamera = cam;

        _renderer.OnLoad();

        World.OnStart();

        _editor = new ImGuiEditor(this);
    }

    public static void Start()
    {
        Instance.Run();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        World.DestroyAll();
        _renderer.OnUnload();
        Assets.UnloadAll();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);

        _editor.WindowResized(ClientSize.X, ClientSize.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        World.ProcessDestroy();

        foreach (var e in World.Updatables)
        {
            e.Update((float)args.Time);
        }

        _editor.Update((float)args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        // Rendering
        _renderer.Render(World);

        _editor.Render((float)args.Time);

        SwapBuffers();
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);

        _editor.PressChar((char)e.Unicode);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _editor.MouseScroll(e.Offset);
    }
}
