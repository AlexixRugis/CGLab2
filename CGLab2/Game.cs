using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;
using Dear_ImGui_Sample;
using ImGuiNET;
using System.IO;

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

        Assets.LoadShader("ShaderTexLit",
            "Resources/Shaders/vertLit.glsl",
            "Resources/Shaders/fragLit.glsl");

        Assets.LoadTexture("TexCat", "Resources/Textures/cat.png", true);
        Assets.LoadTexture("TexSeal", "Resources/Textures/seal.jpg", true);
        Assets.LoadTexture("TexBeach", "Resources/Textures/beach.png", true);
        Assets.LoadTexture("TexPalm", "Resources/Textures/palm.png", true);
        Assets.LoadTexture("Blank", "Resources/Textures/blank.png", false);

        Assets.LoadMesh("MeshQuad", _vertices, _indices, new Mesh.SubMeshInfo[1] { new() { Index = 0, Size = 6 } });
        Assets.LoadMesh("MeshPalm", _vertices2, _indices2, new Mesh.SubMeshInfo[1] { new() { Index = 0, Size = 12 } });

        Entity baloon = new AssimpLoader(Assets).Load(Path.Combine(Directory.GetCurrentDirectory(), "Resources/Models/Baloon/source/viva_baloon.obj"), "", 10f);
        baloon.AddComponent(new SinShakerComponent()
        {
            Speed = 0.5f
        });

        Assets.LoadTexture("TexBaloon1", "Resources/Models/Baloon/textures/Viva_Balloon_Colors_Mat1.jpg", true);
        World.FindEntity("viva_baloon_material_1").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon1"));
        Assets.LoadTexture("TexBaloon2", "Resources/Models/Baloon/textures/Mouth_Mat2.jpg", true);
        World.FindEntity("viva_baloon_material_2").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon2"));
        Assets.LoadTexture("TexBaloon3", "Resources/Models/Baloon/textures/Wood_Bottom_Mat3.jpg", true);
        World.FindEntity("viva_baloon_material_3").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon3"));
        Assets.LoadTexture("TexBaloon4", "Resources/Models/Baloon/textures/Wicker_Mat4.jpg", true);
        World.FindEntity("viva_baloon_material_4").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon4"));
        Assets.LoadTexture("TexBaloon7", "Resources/Models/Baloon/textures/Railing_Leather_Mat_7.jpg", true);
        World.FindEntity("viva_baloon_material_7").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon7"));
        Assets.LoadTexture("TexBaloon13", "Resources/Models/Baloon/textures/light_brown_leatherMat13.jpg", true);
        World.FindEntity("viva_baloon_material_13").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon13"));
        Assets.LoadTexture("TexBaloonSilver", "Resources/Models/Baloon/textures/PropaneSilver4_tanksMat_8.jpg", true);
        World.FindEntity("viva_baloon_silver").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonSilver"));
        World.FindEntity("viva_baloon_tanccover").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonSilver"));
        Assets.LoadTexture("TexBaloonBALENV", "Resources/Models/Baloon/textures/balenv_blue.jpg", true);
        World.FindEntity("viva_baloon_balenv").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonBALENV"));
        Assets.LoadTexture("TexBaloonRED", "Resources/Models/Baloon/textures/RED.jpg", true);
        World.FindEntity("viva_baloon_RED").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonRED"));
        World.FindEntity("viva_baloon_RedBurnerSteel").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonRED"));
        Assets.LoadTexture("TexBaloon17", "Resources/Models/Baloon/textures/Scoop_FabricMat_17.jpg", true);
        World.FindEntity("viva_baloon_material_17").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon17"));
        Assets.LoadTexture("TexBaloonCables", "Resources/Models/Baloon/textures/SteelCables36_40_41_42.jpg", true);
        World.FindEntity("viva_baloon_material_36").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        World.FindEntity("viva_baloon_material_40").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        World.FindEntity("viva_baloon_material_41").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        World.FindEntity("viva_baloon_material_42").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));

        Entity potion = new AssimpLoader(Assets).Load(Path.Combine(Directory.GetCurrentDirectory(), "Resources/Models/potion1.fbx"), "");
        potion.Transform.LocalPosition = new Vector3(5.0f, 0.0f, -2f);
        potion.AddComponent(new RotatorComponent() { Speed = 1.0f });

        Entity floor = World.CreateEntity("Floor");
        floor.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshQuad"),
            Materials = new List<Material>() { new LitTexturedMaterial(Assets.GetTexture("TexBeach")) }
        });
        floor.Transform.LocalRotation = Quaternion.FromEulerAngles(0.5f * MathF.PI, 0.0f, 0.0f);
        floor.Transform.LocalPosition += Vector3.UnitY * -0.5f;
        floor.Transform.LocalScale = new Vector3(15.0f, 10.0f, 1.0f);

        Entity palm1 = World.CreateEntity("Palm1");
        palm1.Transform.LocalPosition -= Vector3.UnitX * 2.0f;
        palm1.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshPalm"),
            Materials = new List<Material>() { new LitTexturedMaterial(Assets.GetTexture("TexPalm")) }
        });

        Entity palm2 = World.CreateEntity("Palm2");
        palm2.Transform.LocalPosition += Vector3.UnitX * 4.0f;
        palm2.AddComponent(new StaticMeshComponent()
        {
            Mesh = Assets.GetMesh("MeshPalm"),
            Materials = new List<Material>() { new LitTexturedMaterial(Assets.GetTexture("TexPalm")) }
        });

        Entity palm3 = World.CreateEntity("Palm3");
        palm3.Transform.LocalPosition -= Vector3.UnitX * 1.0f;
        palm3.Transform.LocalPosition -= Vector3.UnitZ * 2.0f;
        palm3.AddComponent(new StaticMeshComponent()
        {
            Mesh = Assets.GetMesh("MeshPalm"),
            Materials = new List<Material>() { new LitTexturedMaterial(Assets.GetTexture("TexPalm")) }
        });

        Entity cat = World.CreateEntity("Cat");
        cat.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshQuad"),
            Materials = new List<Material>() { new LitTexturedMaterial(Assets.GetTexture("TexCat")) }
        });
        cat.AddComponent(new RotatorComponent() { Speed = 1.0f });

        Entity seal = World.CreateEntity("Seal", cat);
        seal.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshQuad"),
            Materials = new List<Material>() { new LitTexturedMaterial(Assets.GetTexture("TexSeal")) }
        });
        seal.AddComponent(new RotatorComponent() { Speed = 3.0f });
        seal.Transform.LocalPosition += Vector3.UnitX * 2.0f;

        Entity cameraEntity = World.CreateEntity("Camera");
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
