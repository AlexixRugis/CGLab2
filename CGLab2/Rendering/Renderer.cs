using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class Renderer
{
    struct DrawRequest
    {
        public Mesh Mesh;
        public Material Material;
        public Matrix4 ModelMatrix;
        public int SubMeshIndex;
    }

    private List<DrawRequest> _drawRequests = new List<DrawRequest>();

    public void OnLoad()
    {
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public void OnUnload() { }

    public void Render(World world)
    {
        Camera camera = world.CurrentCamera;
        if (camera == null) return;

        _drawRequests.Clear();
        
        foreach (var r in world.Renderers)
        {
            r.Render(this, camera);
        }

        GL.ClearColor(camera.ClearColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Matrix4 view = camera.Entity.Transform.WorldToLocal;
        Matrix4 proj = camera.GetProjectionMatrix();

        for (int i = 0; i < _drawRequests.Count; i++)
        {
            DrawRequest dr = _drawRequests[i];

            Mesh mesh = dr.Mesh;
            mesh.Bind();
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

    public void DrawMesh(Mesh mesh, Material material, int subMeshIndex, Matrix4 transform)
    {
        _drawRequests.Add(new DrawRequest { 
            Mesh = mesh, ModelMatrix = transform, 
            Material = material, SubMeshIndex = subMeshIndex });
    }
}
