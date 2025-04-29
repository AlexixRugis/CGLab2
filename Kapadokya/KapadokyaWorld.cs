
using OpenTK.Mathematics;
using System.Drawing;

public class KapadokyaWorld : World
{
    public override void LoadResources()
    {
        Assets.LoadTexture("TexCat", "Resources/Textures/cat.png", true);
        Assets.LoadTexture("TexSeal", "Resources/Textures/seal.jpg", true);

        Assets.LoadEntity("Kapadokya", "Resources/Models/Kapadokya/muze1M.obj");
        Assets.LoadTexture("KapadokyaTex", "Resources/Models/Kapadokya/muze1M.jpg", true);

        Assets.LoadTexture("TexOiia", "Resources/Models/Oiia/Muchkin2_BaseColor.png", true);
        Assets.LoadEntity("PrefabOiia", "Resources/Models/Oiia/OiiaioooooiaiFin.fbx");

        Assets.LoadEntity("PrefabPalm", "Resources/Models/Palm/Palm_4_1.fbx", 0.002f);
        Assets.LoadTexture("TreeLeaf", "Resources/Models/Palm/LeafMap.png", true);

        Assets.LoadTexture("TreeTrunk", "Resources/Models/Palm/Trunk'_Atlas.png", true);

        Assets.LoadEntity("PrefabBaloon", "Resources/Models/Baloon/source/viva_baloon.obj", 10.0f);
        Assets.LoadTexture("TexBaloon1", "Resources/Models/Baloon/textures/Viva_Balloon_Colors_Mat1.jpg", true);
        Assets.LoadTexture("TexBaloon2", "Resources/Models/Baloon/textures/Mouth_Mat2.jpg", true);
        Assets.LoadTexture("TexBaloon3", "Resources/Models/Baloon/textures/Wood_Bottom_Mat3.jpg", true);
        Assets.LoadTexture("TexBaloon4", "Resources/Models/Baloon/textures/Wicker_Mat4.jpg", true);
        Assets.LoadTexture("TexBaloon7", "Resources/Models/Baloon/textures/Railing_Leather_Mat_7.jpg", true);
        Assets.LoadTexture("TexBaloon13", "Resources/Models/Baloon/textures/light_brown_leatherMat13.jpg", true);
        Assets.LoadTexture("TexBaloonSilver", "Resources/Models/Baloon/textures/PropaneSilver4_tanksMat_8.jpg", true);
        Assets.LoadTexture("TexBaloonBALENV", "Resources/Models/Baloon/textures/balenv_blue.jpg", true);
        Assets.LoadTexture("TexBaloonRED", "Resources/Models/Baloon/textures/RED.jpg", true);
        Assets.LoadTexture("TexBaloon17", "Resources/Models/Baloon/textures/Scoop_FabricMat_17.jpg", true);
        Assets.LoadTexture("TexBaloonCables", "Resources/Models/Baloon/textures/SteelCables36_40_41_42.jpg", true);

        Assets.LoadCubemap("Skybox", new string[] {
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_2_Left+X.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_3_Right-X.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_4_Up+Y.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_5_Down-Y.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_0_Front+Z.png",
        "Resources/Textures/Skybox/Epic_GloriousPink_Cam_1_Back-Z.png",}, true);
    }

    public override void UnloadResources()
    {
    }

    public override void LoadEntities()
    {
        // Kapadokya
        Entity kapadokyaPrefab = Assets.GetEntity("Kapadokya");
        StaticMeshComponent kapadokyaMesh = kapadokyaPrefab.GetChild("defaultobject").GetComponent<StaticMeshComponent>();
        kapadokyaMesh.Materials[0] = new UnlitTexturedMaterial(Assets.GetTexture("KapadokyaTex"));

        Entity k = kapadokyaPrefab.Clone();
        k.Transform.LocalPosition = new Vector3(0.0f, -295.0f, 20.0f);
        k.Transform.LocalRotation = Quaternion.FromEulerAngles(-0.5f * MathF.PI, 0.0f, 0.25f * MathF.PI);
        k.Transform.LocalScale = new Vector3(0.25f, 0.25f, 0.25f);

        // oiiacat
        Entity oiia = Assets.GetEntity("PrefabOiia").Clone();
        oiia.GetChild("Muchkin1.002").GetComponent<StaticMeshComponent>()
            .Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexOiia"));
        oiia.Transform.LocalPosition = new Vector3(12.0f, -1.2f, 5.0f);
        oiia.Transform.LocalRotation = Quaternion.FromEulerAngles(0.0f, -0.1f, 0.0f);

        // palms
        Entity palmPrefab = Assets.GetEntity("PrefabPalm");
        palmPrefab.Transform.Children[0].Entity.GetComponent<StaticMeshComponent>().Materials = new List<Material>()
        {
            new LitTexturedMaterial(Assets.GetTexture("TreeTrunk")),
            new LitTexturedMaterial(Assets.GetTexture("TreeLeaf")),
        };

        Entity palmM = palmPrefab.Clone();
        palmM.Transform.LocalPosition = new Vector3(-2.0f, -3.5f, 2.0f);

        Entity palmM2 = palmM.Clone();
        palmM2.Transform.LocalPosition = new Vector3(4.0f, -3.0f, 0.0f);

        Entity palmM3 = palmM.Clone();
        palmM3.Transform.LocalPosition = new Vector3(-1.0f, -3.5f, -2.0f);

        // baloons
        Entity baloonPrefab = Assets.GetEntity("PrefabBaloon");

        baloonPrefab.AddComponent(new SinShakerComponent()
        {
            Speed = 0.5f
        });

        baloonPrefab.GetChild("viva_baloon_material_1").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon1"));
        baloonPrefab.GetChild("viva_baloon_material_2").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon2"));
        baloonPrefab.GetChild("viva_baloon_material_3").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon3"));
        baloonPrefab.GetChild("viva_baloon_material_4").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon4"));
        baloonPrefab.GetChild("viva_baloon_material_7").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon7"));
        baloonPrefab.GetChild("viva_baloon_material_13").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon13"));
        baloonPrefab.GetChild("viva_baloon_silver").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonSilver"));
        baloonPrefab.GetChild("viva_baloon_tanccover").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonSilver"));
        baloonPrefab.GetChild("viva_baloon_balenv").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonBALENV"));
        baloonPrefab.GetChild("viva_baloon_RED").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonRED"));
        baloonPrefab.GetChild("viva_baloon_RedBurnerSteel").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonRED"));
        baloonPrefab.GetChild("viva_baloon_material_17").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloon17"));
        baloonPrefab.GetChild("viva_baloon_material_36").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        baloonPrefab.GetChild("viva_baloon_material_40").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        baloonPrefab.GetChild("viva_baloon_material_41").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));
        baloonPrefab.GetChild("viva_baloon_material_42").GetComponent<StaticMeshComponent>().Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexBaloonCables"));

        Entity baloon = baloonPrefab.Clone();
        Entity baloon2 = baloonPrefab.Clone();
        baloon2.GetComponent<SinShakerComponent>().Delta = 2.0f;
        baloon2.GetComponent<SinShakerComponent>().Speed = 0.25f;
        baloon2.Transform.LocalPosition = new Vector3(10.0f, 10.0f, -15.0f);
        Entity baloon3 = baloonPrefab.Clone();
        baloon3.Transform.LocalPosition = new Vector3(10.0f, 15.0f, 15.0f);
        baloon3.GetComponent<SinShakerComponent>().Delta = 0.5f;

        // sphere
        Entity testSphere = Assets.GetEntity("PrefabSphere").Clone();
        testSphere.Transform.LocalPosition = new Vector3(0.0f, 3.0f, 0.0f);

        // cubecat
        Entity cat = Assets.GetEntity("PrefabCube").Clone();
        StaticMeshComponent catRenderer = cat.GetComponent<StaticMeshComponent>();
        catRenderer.Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexCat"));
        cat.AddComponent(new RotatorComponent() { Speed = 1.0f });
        cat.Transform.LocalPosition = new Vector3(1.0f, -2.5f, 0.0f);

        // seal
        Entity seal = Assets.GetEntity("PrefabQuad").Clone();
        StaticMeshComponent sealRenderer = seal.GetComponent<StaticMeshComponent>();
        sealRenderer.Materials[0] = new LitTexturedMaterial(Assets.GetTexture("TexSeal"));
        seal.AddComponent(new RotatorComponent() { Speed = 3.0f });
        seal.Transform.LocalPosition += Vector3.UnitX * 2.0f;
        seal.Transform.SetParent(cat.Transform);
        Entity seal2 = seal.Clone();
        seal2.Transform.LocalPosition = new Vector3(-2.0f, 0.0f, 0.0f);
        seal2.Transform.SetParent(cat.Transform);

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
        cam.Skybox = new CubemapMaterial(Assets.GetCubemap("Skybox"));
        cam.IsOrthograthic = false;
        cam.ClearColor = Color.SkyBlue;
        cam.Entity.Transform.LocalPosition = new Vector3(-10.0f, 10.0f, 5.0f);
    }
}
