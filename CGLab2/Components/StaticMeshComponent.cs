public class StaticMeshComponent : Component, IRenderable
{
    public Mesh? Mesh { get; set; }
    public Texture? Texture { get; set; }

    public void Render(Renderer renderer, Camera camera)
    {
        if (Mesh != null && Texture != null)
            renderer.DrawMesh(Mesh, Texture, Transform.LocalToWorld);
    }
}