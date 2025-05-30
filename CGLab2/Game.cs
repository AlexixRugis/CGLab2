﻿using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using System.Diagnostics;

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

    private World? _nextWorld = null;
    public World? World { get; private set; } = null;
    public Renderer Renderer { get; } = new Renderer();

    private Game() : base(GameWindowSettings.Default,
        new NativeWindowSettings()
        {
            ClientSize = new Vector2i(_width, _height),
            Title = _title,
            Flags = ContextFlags.Debug,
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

    public void LoadWorld(World world)
    {
        if (world == null) throw new ArgumentNullException(nameof(world));

        _nextWorld = world;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);

        Assets.LoadShader("ShaderTexUnlit",
            "CoreResources/Shaders/vert.glsl",
            "CoreResources/Shaders/frag.glsl");

        Assets.LoadShader("ShaderTexLit",
            "CoreResources/Shaders/vertLit.glsl",
            "CoreResources/Shaders/fragLit.glsl");

        Assets.LoadShader("ShaderSkybox",
            "CoreResources/Shaders/vertSkybox.glsl",
            "CoreResources/Shaders/fragSkybox.glsl");

        Assets.LoadShader("ShaderFullscreen",
            "CoreResources/Shaders/vertFullscreen.glsl",
            "CoreResources/Shaders/fragFullscreen.glsl");

        Assets.LoadTexture("Blank", "CoreResources/Textures/blank.png", false);

        Primitives.Load(Assets);

        Renderer.OnLoad();

        _editor = new ImGuiEditor(this);
    }

    public static void Start(World startWorld)
    {
        Instance.LoadWorld(startWorld);
        Instance.Run();
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        UnloadCurrentWorld();

        Renderer.OnUnload();
        Assets.UnloadAll();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        Renderer.OnResized(ClientSize.X, ClientSize.Y);
        _editor.WindowResized(ClientSize.X, ClientSize.Y);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (_nextWorld != null)
        {
            UnloadCurrentWorld();
            LoadNextWorld();
        }

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
        Renderer.Render(World);

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

    private static void DebugCallback(DebugSource source, DebugType type, int id,
                                  DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
        string msg = Marshal.PtrToStringAnsi(message, length);
        Console.WriteLine($"[OpenGL DEBUG] {type} {severity} | {msg}");
    }

    private void UnloadCurrentWorld()
    {
        if (World == null) return;

        World.DestroyAll();
        World.ProcessDestroy();
        World.UnloadResources();
        World = null;
    }

    private void LoadNextWorld()
    {
        World = _nextWorld;
        _nextWorld = null;

        World.Game = this;
        World.LoadResources();
        World.LoadEntities();
        World.OnStart();
    }
}
