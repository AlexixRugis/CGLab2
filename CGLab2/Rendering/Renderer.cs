using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class Renderer
{
    public uint DrawCalls { get; private set; } = 0;
    public event Action? PostRenderCallback;
    public event Action? ViewportSizeChangeCallback;

    private static readonly Vertex[] _fullscreenVertices =
    {
        new Vertex() { Position = new Vector3(1.0f, 1.0f, 0.0f), UV = new Vector2(1.0f,1.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(1.0f, -1.0f, 0.0f), UV = new Vector2(1.0f,0.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(-1.0f, -1.0f, 0.0f), UV = new Vector2(0.0f,0.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) },
        new Vertex() { Position = new Vector3(-1.0f, 1.0f, 0.0f), UV = new Vector2(0.0f,1.0f), Normal = new Vector3(0.0f, 0.0f, -1.0f) }
    };

    private static readonly uint[] _fullscreenIndices =
    {
        0, 1, 3,
        1, 2, 3
    };

    private float[] _skyboxVertices = {
        // positions          
        -1.0f,  1.0f, -1.0f,
        -1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f, -1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,

        -1.0f, -1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f, -1.0f,  1.0f,
        -1.0f, -1.0f,  1.0f,

        -1.0f,  1.0f, -1.0f,
         1.0f,  1.0f, -1.0f,
         1.0f,  1.0f,  1.0f,
         1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f,  1.0f,
        -1.0f,  1.0f, -1.0f,

        -1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f, -1.0f,
         1.0f, -1.0f, -1.0f,
        -1.0f, -1.0f,  1.0f,
         1.0f, -1.0f,  1.0f
    };

    private int _skyboxVAO;
    private int _skyboxVBO;

    private Mesh _fullscreenMesh;

    struct DrawRequest
    {
        public Material Material;
        public Matrix4 ModelMatrix;
        public int SubMeshIndex;
    }

    private Dictionary<Mesh, List<DrawRequest>> _drawRequests = new Dictionary<Mesh, List<DrawRequest>>();

    public void OnLoad()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Lequal);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        //GL.Enable(EnableCap.CullFace);
        //GL.CullFace(CullFaceMode.Back);

        _skyboxVAO = GL.GenVertexArray();
        GL.BindVertexArray(_skyboxVAO);
        _skyboxVBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _skyboxVBO);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * _skyboxVertices.Length, _skyboxVertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        _fullscreenMesh = new Mesh(_fullscreenVertices, _fullscreenIndices, new Mesh.SubMeshInfo[]
        {
            new Mesh.SubMeshInfo() { Index = 0, Size = 6 }
        });
    }

    public void OnUnload()
    {
        GL.DeleteVertexArray(_skyboxVAO);
        GL.DeleteBuffer(_skyboxVBO);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void Render(World world)
    {
        Camera camera = world.CurrentCamera;
        if (camera == null) return;

        if (camera.Target != null)
            camera.Target.Bind();

        _drawRequests.Clear();
        DrawCalls = 0;
        
        foreach (var r in world.Renderers)
        {
            r.Render(this, camera);
        }

        Matrix4 view = camera.Entity.Transform.WorldToLocal;
        Matrix4 proj = camera.GetProjectionMatrix();

        GL.ClearColor(camera.ClearColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var kv in _drawRequests)
        {
            Mesh mesh = kv.Key;
            mesh.Bind();

            for (int i = 0; i < kv.Value.Count; i++)
            {
                DrawRequest dr = kv.Value[i];
                Mesh.SubMeshInfo subMesh = mesh.SubMeshes[dr.SubMeshIndex];

                dr.Material.Use();
                Shader s = dr.Material.Shader;
                s.SetMatrix("_View", ref view);
                s.SetMatrix("_Projection", ref proj);
                s.SetMatrix("_Model", ref dr.ModelMatrix);

                GL.DrawElements(PrimitiveType.Triangles, subMesh.Size, DrawElementsType.UnsignedInt, subMesh.Index);
                DrawCalls++;
            }
        }

        // Skybox
        if (camera.Skybox != null)
        {
            CubemapMaterial mat = camera.Skybox;
            Shader s = mat.Shader;
            Matrix4 skyboxView = view.ClearTranslation();

            mat.Use();
            s.SetMatrix("_View", ref skyboxView);
            s.SetMatrix("_Projection", ref proj);
            GL.BindVertexArray(_skyboxVAO);
            GL.DrawArrays(BeginMode.Triangles, 0, 36);
        }

        if (camera.Target != null)
            camera.Target.Unbind();

        PostRenderCallback?.Invoke();
    }

    public void DrawMesh(Mesh mesh, Material material, int subMeshIndex, Matrix4 transform)
    {
        if (!_drawRequests.ContainsKey(mesh))
        {
            _drawRequests.Add(mesh, new List<DrawRequest>());
        }

        _drawRequests[mesh].Add(new DrawRequest { 
            ModelMatrix = transform, 
            Material = material, SubMeshIndex = subMeshIndex });
    }

    public void Blit(Texture? tex, Framebuffer framebuffer, Material material)
    {
        GL.Disable(EnableCap.DepthTest);
        framebuffer.Bind();
        _fullscreenMesh.Bind();
        material.Use();
        if (tex != null) 
            tex.Bind(TextureUnit.Texture0);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        framebuffer.Unbind();
        GL.Enable(EnableCap.DepthTest);
    }

    public void Blit(Texture? tex, Material material)
    {
        GL.Disable(EnableCap.DepthTest);

        _fullscreenMesh.Bind();
        material.Use();
        if (tex != null)
            tex.Bind(TextureUnit.Texture0);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        GL.Enable(EnableCap.DepthTest);
    }

    internal void OnResized(int x, int y)
    {
        GL.Viewport(0, 0, x, y);
        ViewportSizeChangeCallback?.Invoke();
    }
}
