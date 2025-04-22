using OpenTK.Mathematics;
using System.Drawing;

public class RayTracingWorld : World
{

    public override void LoadResources()
    {
        Assets.LoadShader("ShaderRT",
            "Resources/Shaders/vertRT.glsl",
            "Resources/Shaders/fragRT.glsl");
    }

    public override void UnloadResources()
    {
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
        cam.Target = new Framebuffer(1280, 720);
        //cam.Skybox = new CubemapMaterial(Assets.GetCubemap("Skybox"));
        cam.IsOrthograthic = false;
        cam.ClearColor = Color.SkyBlue;
        cam.Entity.Transform.LocalPosition = new Vector3(-0.104f, 3.624f, 8.414f);
        cam.Entity.Transform.LocalRotation = Quaternion.FromEulerAngles(-0.174f, 0.118f, 0.021f);

        // fullscreen
        Entity screen = CreateEntity("RTRenderer");
        screen.AddComponent(new RTRendererComponent());
    }
}
