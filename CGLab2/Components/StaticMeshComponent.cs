
public class StaticMeshComponent : Component, IRenderable
{
    public Mesh? Mesh { get; set; }
    public Material? Material { get; set; }

    public void Render(Renderer renderer, Camera camera)
    {
        if (Mesh != null && Material != null)
        {
            for (int i = 0; i < Mesh.SubMeshes.Count; i++)
            {
                renderer.DrawMesh(Mesh, Material, i, Transform.LocalToWorld);
            }
        }
    }
}