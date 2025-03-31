using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class Renderer
{
    struct DrawRequest
    {
        public Mesh Mesh;
        public Matrix4 ModelMatrix;
    }

    private Shader _shader;

    private List<DrawRequest> _drawRequests = new List<DrawRequest>();

    public void OnLoad()
    {
        GL.Enable(EnableCap.Texture2D);
        GL.Disable(EnableCap.Blend);
        GL.Disable(EnableCap.Lighting);

        string vpath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Shaders/vert.glsl");
        string fpath = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Shaders/frag.glsl");
        string vtext = File.ReadAllText(vpath);
        string ftext = File.ReadAllText(fpath);
        _shader = new Shader(vtext, ftext);
    }

    public void OnUnload()
    {
        _shader.Dispose();
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

        GL.ClearColor(camera.ClearColor);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader.Bind();
        Matrix4 view = camera.Entity.Transform.WorldToLocal;
        Matrix4 proj = camera.GetProjectionMatrix();
        _shader.SetMatrix("_View", ref view);
        _shader.SetMatrix("_Projection", ref proj);

        for (int i = 0; i < _drawRequests.Count; i++)
        {
            DrawRequest dr = _drawRequests[i];

            _shader.SetMatrix("_Model", ref dr.ModelMatrix);
            dr.Mesh.Bind();
            GL.DrawElements(PrimitiveType.Triangles, dr.Mesh.IndicesCount, DrawElementsType.UnsignedInt, 0);
        }
    }

    public void DrawMesh(Mesh mesh, Matrix4 transform)
    {
        _drawRequests.Add(new DrawRequest { Mesh = mesh, ModelMatrix = transform });
    }
}
