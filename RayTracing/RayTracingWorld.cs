using OpenTK.Mathematics;
using System.Drawing;

public class RayTracingWorld : World
{

    public override void LoadResources()
    {
        Assets.LoadShader("ShaderRT",
            "Resources/Shaders/vertRT.glsl",
            "Resources/Shaders/fragRT.glsl");

        Assets.LoadCubemap("Skybox", new string[] {
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_2_Left+X.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_3_Right-X.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_4_Up+Y.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_5_Down-Y.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_0_Front+Z.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_1_Back-Z.png",}, true);

        Assets.LoadEntity("Teapot", "Resources/Models/teapot.obj", 0.01f, true);
    }

    public override void UnloadResources()
    {
    }

    public override void LoadEntities()
    {
        // camera
        Entity cameraEntity = CreateEntity("Camera");
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
        cam.Entity.Transform.LocalPosition = new Vector3(-0.104f, 3.624f, 8.414f);
        cam.Entity.Transform.LocalRotation = Quaternion.FromEulerAngles(-0.174f, 0.118f, 0.021f);

        // fullscreen
        Entity screen = CreateEntity("RTRenderer");
        screen.AddComponent(new RTRendererComponent());
    }
}
