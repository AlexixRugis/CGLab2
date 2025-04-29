using OpenTK.Mathematics;

public class RTRendererComponent : Component, IUpdatable
{
    private SSBO<RTMaterial.Sphere> _spheres;
    private SSBO<RTMaterial.Vertex> _vertices;
    private SSBO<uint> _indices;
    private SSBO<RTMaterial.MeshInfo> _meshes;


    private RTMaterial _material;
    private FullscreenMaterial _fullscreenMat;

    private Framebuffer _frame1;
    private Framebuffer _frame2;

    private Vector3 _lastPosition;
    private Quaternion _lastRotation;

    public override void OnStart()
    {
        Vector2i clientSize = Game.Instance.ClientSize;
        _frame1 = new Framebuffer(clientSize.X, clientSize.Y);
        _frame2 = new Framebuffer(clientSize.X, clientSize.Y);

        RTMaterial.Sphere[] spheres = new RTMaterial.Sphere[40];
        spheres[0] = new RTMaterial.Sphere()
        {
            Position = new Vector3(1.0f, 1.0f, 0.0f),
            Radius = 1.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(1.0f, 1.0f, 1.0f),
                EmissionStrength = 0.0f,
                Smoothness = 0.8f,
                Metallic = 0.2f
            }
        };

        spheres[1] = new RTMaterial.Sphere()
        {
            Position = new Vector3(3.0f, 0.0f, 0.0f),
            Radius = 1.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(1.0f, 0.0f, 1.0f),
                EmissionStrength = 0.0f,
                Smoothness = 0.5f,
                Metallic = 0.1f
            }
        };

        spheres[2] = new RTMaterial.Sphere()
        {
            Position = new Vector3(-6.0f, 10.0f, 5.0f),
            Radius = 4.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(0.0f, 0.0f, 0.0f),
                EmissionColor = new Vector3(1.0f, 1.0f, 1.0f),
                EmissionStrength = 2.0f,
                Smoothness = 0.0f,
                Metallic = 0.1f
            }
        };

        spheres[3] = new RTMaterial.Sphere()
        {
            Position = new Vector3(3.0f, -10.0f, 0.0f),
            Radius = 10.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(0.0f, 1.0f, 1.0f),
                EmissionStrength = 0.0f,
                Smoothness = 0.0f,
                Metallic = 0.1f
            }
        };

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                spheres[4 + 6 * i + j] = new RTMaterial.Sphere()
                {
                    Position = new Vector3(i * 1.0f, 3.0f + j * 1.0f, 0.0f),
                    Radius = 0.5f,
                    Material = new RTMaterial.Material()
                    {
                        Color = new Vector3(0.839f, 0.917f, 1.0f),
                        EmissionStrength = 0.0f,
                        Smoothness = 0.2f * i,
                        Metallic = 0.2f * j
                    }
                };
            }
        }

        _spheres = new SSBO<RTMaterial.Sphere>(spheres);

        RTMaterial.Vertex[] vertices = new RTMaterial.Vertex[Primitives.CubeVertices.Length];
        for (int i = 0; i <  Primitives.CubeVertices.Length; i++)
            vertices[i].Position = Primitives.CubeVertices[i].Position;
        uint[] indices = Primitives.CubeIndices;

        RTMaterial.MeshInfo[] meshInfos = new RTMaterial.MeshInfo[2];
        meshInfos[0] = new RTMaterial.MeshInfo()
        {
            StartIndex = 0,
            IndexCount = indices.Length,
            Transform = Matrix4.CreateTranslation(-1.0f, 1.0f, -1.5f),
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(1.0f, 1.0f, 0.0f),
                EmissionStrength = 0.0f,
                Smoothness = 1.0f,
                Metallic = 0.9f
            }
        };

        Matrix4 tr2 =
            Matrix4.CreateScale(1.0f, 10.0f, 10.0f) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitY, 1.0f) *
            Matrix4.CreateTranslation(10.0f, 3.0f, -5.0f);

        meshInfos[1] = new RTMaterial.MeshInfo()
        {
            StartIndex = 0,
            IndexCount = indices.Length,
            Transform = tr2,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(1.0f, 0.0f, 0.7843f),
                EmissionStrength = 0.0f,
                Smoothness = 0.8f,
                Metallic = 0.5f
            }
        };

        _vertices = new SSBO<RTMaterial.Vertex>(vertices);
        _indices = new SSBO<uint>(indices);
        _meshes = new SSBO<RTMaterial.MeshInfo>(meshInfos);

        _material = new RTMaterial(Entity.World.CurrentCamera, 
            _spheres, _vertices, _indices, _meshes);
        _fullscreenMat = new FullscreenMaterial();

        Game.Instance.Renderer.PostRenderCallback += PostRenderCallback;
        Game.Instance.Renderer.ViewportSizeChangeCallback += ViewportSizeCallback;
    }

    public override void OnDestroy()
    {
        _spheres.Dispose();
        _vertices.Dispose();
        _indices.Dispose();
        _meshes.Dispose();
        _frame1.Dispose();
        _frame2.Dispose();

        Game.Instance.Renderer.PostRenderCallback -= PostRenderCallback;
        Game.Instance.Renderer.ViewportSizeChangeCallback -= ViewportSizeCallback;
    }

    public override Component Clone()
    {
        return new RTRendererComponent();
    }

    private void PostRenderCallback()
    {
        Renderer r = Game.Instance.Renderer;
        _material.PrevFrame = _frame2.ColorTexture;
        _material.Cubemap = Game.Instance.World.CurrentCamera.Skybox.Texture;
        r.Blit(null, _frame1, _material);
        r.Blit(_frame1.ColorTexture, _material);

        Framebuffer temp = _frame1;
        _frame1 = _frame2;
        _frame2 = temp;
    }

    private void ViewportSizeCallback()
    {
        _material.Frame = 0;
        _frame1.Dispose();
        _frame2.Dispose();

        Vector2i clientSize = Game.Instance.ClientSize;
        _frame1 = new Framebuffer(clientSize.X, clientSize.Y);
        _frame2 = new Framebuffer(clientSize.X, clientSize.Y);
    }

    public void Update(float deltaTime)
    {
        Camera cam = Entity.World.CurrentCamera;
        if (cam.Transform.LocalRotation != _lastRotation ||
            cam.Transform.LocalPosition != _lastPosition)
        {
            _lastPosition = cam.Transform.LocalPosition;
            _lastRotation = cam.Transform.LocalRotation;
            _material.Frame = 0;
        }
        else
        {
            _material.Frame++;
        }

    }
}