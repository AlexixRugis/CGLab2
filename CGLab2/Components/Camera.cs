using OpenTK.Mathematics;

public class Camera : Component
{
    [field: EditorField] public Color4 ClearColor { get; set; } = Color4.Black;
    public CubemapMaterial? Skybox { get; set; } = null;
    public Framebuffer? Target { get; set; } = null;
    [field: EditorField] public bool IsOrthograthic { get; set; } = true;
    [field: EditorField] public float NearPlane { get; set; } = -10f;
    [field: EditorField] public float FarPlane { get; set; } = 10f;
    [field: EditorField] public float FOV { get; set; } = 60f;
    [field: EditorField] public float Width { get; set; } = 5.0f;
    
    public float Height => Width / Screen.Aspect;

    public override void OnStart()
    {
        base.OnStart();

        if (Entity.World.CurrentCamera == null)
            Entity.World.CurrentCamera = this;
        else
            Console.WriteLine("More than one camera in scene.");
    }

    public override Component Clone()
    {
        return new Camera()
        {
            ClearColor = ClearColor,
            Skybox = Skybox,
            Target = Target,
            IsOrthograthic = IsOrthograthic,
            NearPlane = FarPlane,
            FarPlane = FarPlane,
            FOV = FOV,
            Width = Width,
        };
    }

    public Matrix4 GetProjectionMatrix()
    {
        return IsOrthograthic ? Matrix4.CreateOrthographic(Width, Height, NearPlane, FarPlane) :
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), Screen.Aspect, NearPlane, FarPlane); ;
    }
}