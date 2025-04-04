
public class StaticMeshComponent : Component, IRenderable
{
    public Mesh? Mesh { get; set; }
    public List<Material> Materials { get; set; } = new List<Material>();

    public void Render(Renderer renderer, Camera camera)
    {
        if (Mesh != null && Materials != null && Materials.Count == Mesh.SubMeshes.Count)
        {
            for (int i = 0; i < Mesh.SubMeshes.Count; i++)
            {
                renderer.DrawMesh(Mesh, Materials[i], i, Transform.LocalToWorld);
            }
        }
    }
}