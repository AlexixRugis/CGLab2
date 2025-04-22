using OpenTK.Mathematics;
using System.Drawing;

public class RayTracingWorld : World
{

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

    private SSBO<RTMaterial.Sphere> _spheres;
    private Mesh _fullscreenMesh;

    public override void LoadResources()
    {
        Assets.LoadShader("ShaderRT",
            "Resources/Shaders/vertRT.glsl",
            "Resources/Shaders/fragRT.glsl");

        _fullscreenMesh = new Mesh(_fullscreenVertices, _fullscreenIndices, new Mesh.SubMeshInfo[]
        {
            new Mesh.SubMeshInfo() { Index = 0, Size = 6 }
        });
    }

    public override void UnloadResources()
    {
        _fullscreenMesh.Dispose();
        _spheres.Dispose();
    }

    public override void LoadEntities()
    {
        // sphere
        Entity testSphere = Assets.GetEntity("PrefabSphere").Clone();
        testSphere.Transform.LocalPosition = new Vector3(0.0f, 0.0f, 0.0f);

        // light
        Entity lightEntity = CreateEntity("Light");
        Light l = new Light()
        {
            Color = new Color4(248, 237, 203, 255)
        };
        lightEntity.AddComponent(l);
        lightEntity.AddComponent(new StaticMeshComponent()
        {
            Mesh = Assets.GetMesh("MeshQuad"),
            Materials = new List<Material>() { new UnlitTexturedMaterial(Assets.GetTexture("Blank")) }
        });
        lightEntity.Transform.LocalPosition = new Vector3(-5.0F, 10.0f, 100.0f);

        // camera
        Entity cameraEntity = CreateEntity("Camera");
        Camera cam = new Camera();
        cameraEntity.AddComponent(cam);
        cameraEntity.AddComponent(new FreeCameraController() { MovementSpeed = 2.0f });
        cam.FOV = 60;
        cam.NearPlane = 0.1f;
        cam.FarPlane = 100f;
        cam.ClearColor = Color.White;
        //cam.Skybox = new CubemapMaterial(Assets.GetCubemap("Skybox"));
        cam.IsOrthograthic = false;
        cam.ClearColor = Color.SkyBlue;
        cam.Entity.Transform.LocalPosition = new Vector3(0.0f, 0.0f, 5.0f);

        RTMaterial.Sphere[] spheres = new RTMaterial.Sphere[3];
        spheres[0] = new RTMaterial.Sphere()
        {
            Position = new Vector3(1.0f, 1.0f, 0.0f),
            Radius = 1.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f)
            }
        };

        spheres[1] = new RTMaterial.Sphere()
        {
            Position = new Vector3(3.0f, 0.0f, 0.0f),
            Radius = 1.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector4(1.0f, 0.0f, 1.0f, 1.0f)
            }
        };

        spheres[2] = new RTMaterial.Sphere()
        {
            Position = new Vector3(-3.0f, 0.0f, -2.0f),
            Radius = 1.5f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f)
            }
        };

        _spheres = new SSBO<RTMaterial.Sphere>(spheres);

        // fullscreen
        Entity screen = CreateEntity("Screen");
        screen.AddComponent(new StaticMeshComponent()
        {
            Mesh = _fullscreenMesh,
            Materials = new List<Material>() { new RTMaterial(cam,_spheres) }
        });
    }
}
