using OpenTK.Mathematics;
using System.Runtime.InteropServices;

public class RTMaterial : Material
{
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct Material
    {
        [FieldOffset(0)] public Vector3 Color;
        [FieldOffset(12)] private float _pad0;

        [FieldOffset(16)] public Vector3 EmissionColor;
        [FieldOffset(28)] public float EmissionStrength;
        [FieldOffset(32)] public float Smoothness;
        [FieldOffset(36)] public float Metallic;
        [FieldOffset(36)] private Vector2 _pad1;
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct Sphere
    {
        [FieldOffset(0)] public Vector3 Position;
        [FieldOffset(12)] public float Radius;

        [FieldOffset(16)] public Material Material;
    }

    private Camera _camera;
    private SSBO<Sphere> _spheres;

    public uint Frame { get; set; } = 0;
    public Texture PrevFrame { get; set; }
    public CubemapTexture Cubemap { get; set; }
    
    public RTMaterial(Camera camera, SSBO<Sphere> spheres)
    {
        _camera = camera;
        _spheres = spheres;
        Shader = Game.Instance.Assets.GetShader("ShaderRT");
    }

    public override void Use()
    {
        Matrix4 invProj = _camera.GetProjectionMatrix().Inverted();
        Matrix4 invTransform = _camera.Transform.LocalToWorld;
        Vector2 screenSize = Game.Instance.ClientSize;

        Shader.Bind();
        _spheres.Bind(0);

        PrevFrame.Bind(OpenTK.Graphics.OpenGL.TextureUnit.Texture0);
        Cubemap.Bind(OpenTK.Graphics.OpenGL.TextureUnit.Texture1);
        Shader.SetInt("prevFrame", 0);
        Shader.SetInt("_Skybox", 1);
        Shader.SetInt("_SpheresCount", _spheres.Count);
        Shader.SetUInt("_Frame", Frame);
        Shader.SetVector2("_ScreenSize", screenSize);
        Shader.SetMatrix("_CameraToWorld", ref invTransform);
        Shader.SetMatrix("_CameraInverseProjection", ref invProj);
    }
}
