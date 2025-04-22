using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class RTRendererComponent : Component
{
    private SSBO<RTMaterial.Sphere> _spheres;
    private RTMaterial _material;
    private FullscreenMaterial _fullscreenMat;

    private Framebuffer _frame1;
    private Framebuffer _frame2;

    public override void OnStart()
    {
        Vector2i clientSize = Game.Instance.ClientSize;
        _frame1 = new Framebuffer(clientSize.X, clientSize.Y);
        _frame2 = new Framebuffer(clientSize.X, clientSize.Y);

        RTMaterial.Sphere[] spheres = new RTMaterial.Sphere[4];
        spheres[0] = new RTMaterial.Sphere()
        {
            Position = new Vector3(1.0f, 1.0f, 0.0f),
            Radius = 1.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(1.0f, 1.0f, 1.0f),
                EmissionStrength = 0.0f
            }
        };

        spheres[1] = new RTMaterial.Sphere()
        {
            Position = new Vector3(3.0f, 0.0f, 0.0f),
            Radius = 1.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(1.0f, 0.0f, 1.0f),
                EmissionStrength = 0.0f
            }
        };

        spheres[2] = new RTMaterial.Sphere()
        {
            Position = new Vector3(-6.0f, 5.0f, -2.0f),
            Radius = 2.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(0.0f, 0.0f, 0.0f),
                EmissionColor = new Vector3(1.0f, 1.0f, 1.0f),
                EmissionStrength = 20.0f
            }
        };

        spheres[3] = new RTMaterial.Sphere()
        {
            Position = new Vector3(3.0f, -10.0f, 0.0f),
            Radius = 10.0f,
            Material = new RTMaterial.Material()
            {
                Color = new Vector3(0.0f, 1.0f, 1.0f),
                EmissionStrength = 0.0f
            }
        };

        _spheres = new SSBO<RTMaterial.Sphere>(spheres);

        _material = new RTMaterial(Entity.World.CurrentCamera, _spheres);
        _material.AccumFactor = 0.95f;
        _fullscreenMat = new FullscreenMaterial();

        Game.Instance.Renderer.PostRenderCallback += PostRenderCallback;

        Game.Instance.Assets.LoadTexture("TexSeal", "Resources/Textures/seal.jpg", true);
    }

    public override void OnDestroy()
    {
        _spheres.Dispose();
        _frame1.Dispose();
        _frame2.Dispose();

        Game.Instance.Renderer.PostRenderCallback -= PostRenderCallback;
    }

    public override Component Clone()
    {
        return new RTRendererComponent();
    }

    private void PostRenderCallback()
    {
        Renderer r = Game.Instance.Renderer;
        _material.PrevFrame = _frame2.ColorTexture;
        r.Blit(null, _frame1, _material);
        r.Blit(_frame1.ColorTexture, _material);

        Framebuffer temp = _frame1;
        _frame1 = _frame2;
        _frame2 = temp;
    }
}