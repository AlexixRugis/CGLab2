using OpenTK.Mathematics;

public class Camera : Component
{
    public Color4 ClearColor { get; set; }
    public CubemapMaterial? Skybox { get; set; }
    public bool IsOrthograthic { get; set; } = true;
    public float NearPlane { get; set; } = -10f;
    public float FarPlane { get; set; } = 10f;
    public float FOV { get; set; } = 60f;
    public float Width { get; set; }
    public float Height => Width / Screen.Aspect;

    public override Component Clone()
    {
        return new Camera()
        {
            ClearColor = ClearColor,
            Skybox = Skybox,
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