using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class Renderer
{
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

    struct DrawRequest
    {
        public Material Material;
        public Matrix4 ModelMatrix;
        public int SubMeshIndex;
    }

    private Dictionary<Mesh, List<DrawRequest>> _drawRequests = new Dictionary<Mesh, List<DrawRequest>>();

    public void OnLoad()
    {
        GL.Enable(EnableCap.Texture2D);
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
    }

    public void OnUnload()
    {
        GL.DeleteVertexArray(_skyboxVAO);
        GL.DeleteBuffer(_skyboxVBO);
    }

    public void Render(World world)
    {
        Camera camera = world.CurrentCamera;
        if (camera == null) return;

        _drawRequests.Clear();
        
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

                if (GL.GetError() != ErrorCode.NoError)
                {
                    Console.WriteLine("e");
                }
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
}
