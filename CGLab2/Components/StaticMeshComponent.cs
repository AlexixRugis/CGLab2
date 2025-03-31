public class StaticMeshComponent : Component, IRenderable
{
    public Mesh? Mesh { get; set; }

    public void Render(Renderer renderer, Camera camera)
    {
        if (Mesh != null)
            renderer.DrawMesh(Mesh, Transform.LocalToWorld);
    }
}