using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System.Drawing;

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
        new Vertex() { Position = new Vector3(0.5f, 0.5f, 0.0f), UV = new Vector2(1.0f,1.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(0.5f, -0.5f, 0.0f), UV = new Vector2(1.0f,0.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, -0.5f, 0.0f), UV = new Vector2(0.0f,0.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(-0.5f, 0.5f, 0.0f), UV = new Vector2(0.0f,1.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) }
    };

    private readonly uint[] _indices =
    {
        0, 1, 3,
        1, 2, 3
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

        Assets.LoadShader("ShaderSkybox",
            "Resources/Shaders/vertSkybox.glsl",
            "Resources/Shaders/fragSkybox.glsl");

        Assets.LoadCubemap("Skybox", new string[] {
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_2_Left+X.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_3_Right-X.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_4_Up+Y.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_5_Down-Y.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_0_Front+Z.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_1_Back-Z.png",}, true);

        Assets.LoadTexture("TexCat", "Resources/Textures/cat.png", true);
        Assets.LoadTexture("TexSeal", "Resources/Textures/seal.jpg", true);
        Assets.LoadTexture("Blank", "Resources/Textures/blank.png", false);

        Assets.LoadMesh("MeshQuad", _vertices, _indices, new Mesh.SubMeshInfo[1] { new() { Index = 0, Size = 6 } });

        AssimpLoader loader = new AssimpLoader(Assets);

        Assets.LoadEntity("Kapadokya", "Resources/Models/Kapadokya/muze1M.obj");
        Assets.LoadTexture("KapadokyaTex", "Resources/Models/Kapadokya/muze1M.jpg", true);
        Entity kapadokyaPrefab = Assets.GetEntity("Kapadokya");
        kapadokyaPrefab.GetChild("defaultobject").GetComponent<StaticMeshComponent>().Materials[0] = new UnlitTexturedMaterial(Assets.GetTexture("KapadokyaTex"));

        Entity k = kapadokyaPrefab.Clone();
        k.Transform.LocalPosition = new Vector3(0.0f, -295.0f, 20.0f);
        k.Transform.LocalRotation = Quaternion.FromEulerAngles(-0.5f * MathF.PI, 0.0f, 0.25f * MathF.PI);
        k.Transform.LocalScale = new Vector3(0.25f, 0.25f, 0.25f);

        Assets.LoadEntity("PrefabPalm", "Resources/Models/Palm/Palm_4_1.fbx", 0.002f);
        Assets.LoadTexture("TreeLeaf", "Resources/Models/Palm/LeafMap.png", true);
        Assets.LoadTexture("TreeTrunk", "Resources/Models/Palm/Trunk'_Atlas.png", true);
        Entity palmPrefab = Assets.GetEntity("PrefabPalm");
        palmPrefab.Transform.Children[0].Entity.GetComponent<StaticMeshComponent>().Materials = new List<Material>()
        {
            new LitTexturedMaterial(Assets.GetTexture("TreeTrunk")),
            new LitTexturedMaterial(Assets.GetTexture("TreeLeaf")),
        };

        Entity palmM = palmPrefab.Clone();
        palmM.Transform.LocalPosition = new Vector3(-2.0f, -3.5f, 2.0f);

        Entity palmM2 = palmM.Clone();
        palmM2.Transform.LocalPosition = new Vector3(4.0f, -3.0f, 0.0f);

        Entity palmM3 = palmM.Clone();
        palmM3.Transform.LocalPosition = new Vector3(-1.0f, -3.5f, -2.0f);

        Assets.LoadEntity("PrefabBaloon", "Resources/Models/Baloon/source/viva_baloon.obj", 10.0f);
        Entity baloonPrefab = Assets.GetEntity("PrefabBaloon");

        baloonPrefab.AddComponent(new SinShakerComponent()
        {
            Speed = 0.5f
        });

        Assets.LoadTexture("TexBaloon1", "Resources/Models/Baloon/textures/Viva_Balloon_Colors_Mat1.jpg", true);
        baloonPrefab.GetChild("viva_baloon_material_1").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon1"));
        Assets.LoadTexture("TexBaloon2", "Resources/Models/Baloon/textures/Mouth_Mat2.jpg", true);
        baloonPrefab.GetChild("viva_baloon_material_2").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon2"));
        Assets.LoadTexture("TexBaloon3", "Resources/Models/Baloon/textures/Wood_Bottom_Mat3.jpg", true);
        baloonPrefab.GetChild("viva_baloon_material_3").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon3"));
        Assets.LoadTexture("TexBaloon4", "Resources/Models/Baloon/textures/Wicker_Mat4.jpg", true);
        baloonPrefab.GetChild("viva_baloon_material_4").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon4"));
        Assets.LoadTexture("TexBaloon7", "Resources/Models/Baloon/textures/Railing_Leather_Mat_7.jpg", true);
        baloonPrefab.GetChild("viva_baloon_material_7").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon7"));
        Assets.LoadTexture("TexBaloon13", "Resources/Models/Baloon/textures/light_brown_leatherMat13.jpg", true);
        baloonPrefab.GetChild("viva_baloon_material_13").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon13"));
        Assets.LoadTexture("TexBaloonSilver", "Resources/Models/Baloon/textures/PropaneSilver4_tanksMat_8.jpg", true);
        baloonPrefab.GetChild("viva_baloon_silver").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonSilver"));
        baloonPrefab.GetChild("viva_baloon_tanccover").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonSilver"));
        Assets.LoadTexture("TexBaloonBALENV", "Resources/Models/Baloon/textures/balenv_blue.jpg", true);
        baloonPrefab.GetChild("viva_baloon_balenv").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonBALENV"));
        Assets.LoadTexture("TexBaloonRED", "Resources/Models/Baloon/textures/RED.jpg", true);
        baloonPrefab.GetChild("viva_baloon_RED").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonRED"));
        baloonPrefab.GetChild("viva_baloon_RedBurnerSteel").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonRED"));
        Assets.LoadTexture("TexBaloon17", "Resources/Models/Baloon/textures/Scoop_FabricMat_17.jpg", true);
        baloonPrefab.GetChild("viva_baloon_material_17").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon17"));
        Assets.LoadTexture("TexBaloonCables", "Resources/Models/Baloon/textures/SteelCables36_40_41_42.jpg", true);
        baloonPrefab.GetChild("viva_baloon_material_36").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        baloonPrefab.GetChild("viva_baloon_material_40").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        baloonPrefab.GetChild("viva_baloon_material_41").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        baloonPrefab.GetChild("viva_baloon_material_42").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));

        Entity baloon = baloonPrefab.Clone();
        Entity baloon2 = baloonPrefab.Clone();
        baloon2.GetComponent<SinShakerComponent>().Delta = 2.0f;
        baloon2.GetComponent<SinShakerComponent>().Speed = 0.25f;
        baloon2.Transform.LocalPosition = new Vector3(10.0f, 10.0f, -15.0f);
        Entity baloon3 = baloonPrefab.Clone();
        baloon3.Transform.LocalPosition = new Vector3(10.0f, 15.0f, 15.0f);
        baloon3.GetComponent<SinShakerComponent>().Delta = 0.5f;

        Entity cat = World.CreateEntity("Cat");
        cat.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshQuad"),
            Materials = new List<Material>() { new LitTexturedMaterial(Assets.GetTexture("TexCat")) }
        });
        cat.AddComponent(new RotatorComponent() { Speed = 1.0f });
        cat.Transform.LocalPosition = new Vector3(1.0f, -2.5f, 0.0f);

        Entity seal = World.CreateEntity("Seal", cat);
        seal.AddComponent(new StaticMeshComponent() {
            Mesh = Assets.GetMesh("MeshQuad"),
            Materials = new List<Material>() { new LitTexturedMaterial(Assets.GetTexture("TexSeal")) }
        });
        seal.AddComponent(new RotatorComponent() { Speed = 3.0f });
        seal.Transform.LocalPosition += Vector3.UnitX * 2.0f;

        Entity lightEntity = World.CreateEntity("Light");
        Light l = new Light()
        {
            Color = new Color4(248, 237, 203, 255)
        };
        lightEntity.AddComponent(l);
        //lightEntity.AddComponent(new StaticMeshComponent()
        //{
        //    Mesh = Assets.GetMesh("MeshQuad"),
        //    Materials = new List<Material>() { new UnlitTexturedMaterial(Assets.GetTexture("Blank")) }
        //});
        lightEntity.Transform.LocalPosition = new Vector3(-5.0F, 10.0f, 100.0f);
        World.Light = l;

        Entity cameraEntity = World.CreateEntity("Camera");
        Camera cam = new Camera();
        cameraEntity.AddComponent(cam);
        cameraEntity.AddComponent(new FreeCameraController() { MovementSpeed = 2.0f });
        cam.FOV = 60;
        cam.NearPlane = 0.1f;
        cam.FarPlane = 100f;
        cam.ClearColor = Color.White;
        cam.Skybox = new CubemapMaterial(Assets.GetCubemap("Skybox"));
        cam.IsOrthograthic = false;
        cam.ClearColor = Color.SkyBlue;
        cam.Entity.Transform.LocalPosition = new Vector3(-10.0f, 10.0f, 5.0f);

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
