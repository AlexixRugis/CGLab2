using OpenTK.Mathematics;

public class RTRendererComponent : Component, IUpdatable
{
    [EditorField] public int Bounces = 4;
    [EditorField] public int RaysPerPixel = 2;

    private SSBO<RTMaterial.Sphere> _spheres;
    private SSBO<RTMaterial.Vertex> _vertices;
    private SSBO<uint> _indices;
    private SSBO<RTMaterial.MeshInfo> _meshes;
    private SSBO<BVHNode> _nodes;
    private SSBO<RTMaterial.Material> _materials;

    private RTMaterial _material;

    private Framebuffer _frame1;
    private Framebuffer _frame2;

    private Vector3 _lastPosition;
    private Quaternion _lastRotation;

    public override void OnStart()
    {
        Vector2i clientSize = Game.Instance.ClientSize;
        _frame1 = new Framebuffer(clientSize.X, clientSize.Y);
        _frame2 = new Framebuffer(clientSize.X, clientSize.Y);

        List<RTMaterial.Material> mats = new List<RTMaterial.Material>();

        RTMaterial.Sphere[] spheres = new RTMaterial.Sphere[40];
        mats.Add(new RTMaterial.Material()
        {
            Color = new Vector3(1.0f, 1.0f, 1.0f),
            Smoothness = 0.8f,
            Metallic = 0.2f
        });
        spheres[0] = new RTMaterial.Sphere()
        {
            Position = new Vector3(1.0f, 1.0f, 0.0f),
            Radius = 1.0f,
            MaterialIndex = (uint)(mats.Count - 1)
        };

        mats.Add(new RTMaterial.Material()
        {
            Color = new Vector3(1.0f, 0.0f, 1.0f),
            Smoothness = 0.5f,
            Metallic = 0.1f
        });
        spheres[1] = new RTMaterial.Sphere()
        {
            Position = new Vector3(3.0f, 0.0f, 0.0f),
            Radius = 1.0f,
            MaterialIndex = (uint)(mats.Count - 1)
        };

        mats.Add(new RTMaterial.Material()
        {
            Color = new Vector3(0.0f, 0.0f, 0.0f),
            EmissionColor = new Vector3(3.0f, 3.0f, 3.0f),
            Smoothness = 0.0f,
            Metallic = 0.1f
        });
        spheres[2] = new RTMaterial.Sphere()
        {
            Position = new Vector3(-6.0f, 10.0f, 5.0f),
            Radius = 4.0f,
            MaterialIndex = (uint)(mats.Count - 1)
        };

        mats.Add(new RTMaterial.Material()
        {
            Color = new Vector3(0.0f, 1.0f, 1.0f),
            Smoothness = 0.0f,
            Metallic = 0.1f
        });
        spheres[3] = new RTMaterial.Sphere()
        {
            Position = new Vector3(3.0f, -10.0f, 0.0f),
            Radius = 10.0f,
            MaterialIndex = (uint)(mats.Count - 1)
        };

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                mats.Add(new RTMaterial.Material()
                {
                    Color = new Vector3(0.839f, 0.917f, 1.0f),
                    Smoothness = 0.2f * i,
                    Metallic = 0.2f * j
                });
                spheres[4 + 6 * i + j] = new RTMaterial.Sphere()
                {
                    Position = new Vector3(i * 1.0f, 3.0f + j * 1.0f, 0.0f),
                    Radius = 0.5f,
                    MaterialIndex = (uint)(mats.Count - 1)
                };
            }
        }

        _spheres = new SSBO<RTMaterial.Sphere>(spheres);


        Mesh teapotMesh = Game.Instance.Assets.GetEntity("Teapot")
            .GetChild("teapot").GetComponent<StaticMeshComponent>().Mesh;

        BVHMesh bvhMesh = new BVHMesh(teapotMesh.Vertices, teapotMesh.Indices, 32);
        BVHMesh bvhMesh2 = new BVHMesh(Primitives.CubeVertices, Primitives.CubeIndices, 4);

        List<Vertex> allVertices = new List<Vertex>();
        allVertices.AddRange(bvhMesh.Vertices);
        allVertices.AddRange(bvhMesh2.Vertices);

        List<uint> allIndices = new List<uint>();
        allIndices.AddRange(bvhMesh.Indices);
        allIndices.AddRange(bvhMesh2.Indices);

        List<BVHNode> allNodes = new List<BVHNode>();
        allNodes.AddRange(bvhMesh.Nodes);
        allNodes.AddRange(bvhMesh2.Nodes);

        RTMaterial.MeshInfo[] meshInfos = new RTMaterial.MeshInfo[3];

        Matrix4 tr1 =
            Matrix4.CreateScale(0.03f) *
            Matrix4.CreateTranslation(-3.0f, 1.0f, -1.5f);

        mats.Add(new RTMaterial.Material()
        {
            Color = new Vector3(1.0f, 1.0f, 0.0f),
            Smoothness = 1.0f,
            Metallic = 0.9f
        });
        meshInfos[0] = new RTMaterial.MeshInfo()
        {
            NodeIndex = 0,
            Transform = tr1,
            InvTransform = Matrix4.Invert(tr1),
            MaterialIndex = (uint)(mats.Count - 1)
        };

        Matrix4 tr2 =
            Matrix4.CreateScale(0.1f) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitY, -1.0f) *
            Matrix4.CreateTranslation(10.0f, 3.0f, -5.0f);

        mats.Add(new RTMaterial.Material()
        {
            Color = new Vector3(1.0f, 0.0f, 0.7843f),
            Smoothness = 0.8f,
            Metallic = 0.5f
        });
        meshInfos[1] = new RTMaterial.MeshInfo()
        {
            NodeIndex = 0,
            Transform = tr2,
            InvTransform = Matrix4.Invert(tr2),
            MaterialIndex = (uint)(mats.Count - 1)
        };

        Matrix4 tr3 =
            Matrix4.CreateScale(4.0f) *
            Matrix4.CreateFromAxisAngle(Vector3.UnitY, 1.0f) *
            Matrix4.CreateTranslation(-2.0f, 7.0f, -5.0f);

        mats.Add(new RTMaterial.Material()
        {
            Color = new Vector3(1.0f, 1.0f, 1.0f),
            Smoothness = 0.9f,
            Metallic = 0.5f
        });
        meshInfos[2] = new RTMaterial.MeshInfo()
        {
            NodeIndex = bvhMesh.Nodes.Count,
            IndexOffset = (uint)bvhMesh.Indices.Count,
            VertexOffset = (uint)bvhMesh.Vertices.Length,
            Transform = tr3,
            InvTransform = Matrix4.Invert(tr3),
            MaterialIndex = (uint)(mats.Count - 1)
        };

        RTMaterial.Vertex[] vertices = new RTMaterial.Vertex[allVertices.Count];
        for (int i = 0; i < allVertices.Count; i++)
        {
            vertices[i].Position = allVertices[i].Position;
            vertices[i].Normal = allVertices[i].Normal;
        }

        _vertices = new SSBO<RTMaterial.Vertex>(vertices);
        _indices = new SSBO<uint>(allIndices.ToArray());
        _meshes = new SSBO<RTMaterial.MeshInfo>(meshInfos);
        _nodes = new SSBO<BVHNode>(allNodes.ToArray());
        _materials = new SSBO<RTMaterial.Material>(mats.ToArray());

        _material = new RTMaterial(Entity.World.CurrentCamera, 
            _spheres, _vertices, _indices, _meshes, _nodes, _materials);

        Game.Instance.Renderer.PostRenderCallback += PostRenderCallback;
        Game.Instance.Renderer.ViewportSizeChangeCallback += ViewportSizeCallback;
    }

    public override void OnDestroy()
    {
        _spheres.Dispose();
        _vertices.Dispose();
        _indices.Dispose();
        _meshes.Dispose();
        _nodes.Dispose();
        _materials.Dispose();
        _frame1.Dispose();
        _frame2.Dispose();

        Game.Instance.Renderer.PostRenderCallback -= PostRenderCallback;
        Game.Instance.Renderer.ViewportSizeChangeCallback -= ViewportSizeCallback;
    }

    public override Component Clone()
    {
        return new RTRendererComponent()
        {
            Bounces = Bounces,
            RaysPerPixel = RaysPerPixel
        };
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


        _material.Bounces = Bounces;
        _material.RaysPerPixel = RaysPerPixel;
    }
}